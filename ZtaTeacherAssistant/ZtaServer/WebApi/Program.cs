using Microsoft.AspNetCore.Mvc;
using MySql.Data.MySqlClient;
using Newtonsoft.Json.Linq;
using System.Data;
using WebApi.Tables;
using System.Linq;
using System.IO;
using System.Security.Cryptography;
using System.Xml.Linq;

const int MaxSqlResultLength = 100;

const string sqlConnCommand = "server=localhost;user=root;password=Anyangzyl403!;database=ZTA";
bool succ = await Task.Run(() => {
    var ZTAConn = new MySqlConnection(sqlConnCommand);
    try
    {
        ZTAConn.Open();
    }
    catch (Exception)
    {
        return false;
    }
    return ZTAConn.State == ConnectionState.Open;
});

if (!succ)
{
    Console.WriteLine("Failed to connect to database.");
    return;
}

Console.WriteLine("Connection successful.");

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllers()
    .AddNewtonsoftJson();
builder.Services.Configure<Microsoft.AspNetCore.Http.Json.JsonOptions>(options =>
{
    options.SerializerOptions.IncludeFields = true;
});
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

WebApplication app = builder.Build();
app.UseHttpsRedirection();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// app.UseAuthorization();

app.MapControllers();

#region paper
static async Task<List<Paper>?> GetPaper(
    MySqlConnection sqlConn,
    int? pid = null, string? pname = null, string? psource = null, DateTime? pyear_from = null, DateTime? pyear_to = null, int? ptype = null, int? level = null,
    string? orderby = null, bool? desc = null, int? limit = null)
{
    limit ??= 30;
    var sqlQueryStr = "Select * from paper";

    var wheres = new List<string>();
    if (pid is not null) wheres.Add($"pid={pid}");
    if (pname is not null) wheres.Add($"pname='{pname}'");
    if (psource is not null) wheres.Add($"psource='{psource}'");
    if (pyear_from is not null) wheres.Add($"pyear>='{pyear_from?.ToString("yyyy-MM-dd")}'");
    if (pyear_to is not null) wheres.Add($"pyear<='{pyear_to?.ToString("yyyy-MM-dd")}'");
    if (ptype is not null) wheres.Add($"ptype={ptype}");
    if (level is not null) wheres.Add($"level={level}");
    if (wheres.Count > 0)
    {
        sqlQueryStr += " where ";
        sqlQueryStr += string.Join(" and ", wheres);
    }
    sqlQueryStr += $" order by {orderby ?? "pid"}";
    if (desc ?? false) sqlQueryStr += " desc";
    sqlQueryStr += $" limit {Math.Max(Math.Min(limit ?? 0, MaxSqlResultLength), 0)}";

    try
    {
        var sqlCommand = new MySqlCommand(sqlQueryStr, sqlConn);
        var papers = new List<Paper>();
        using (var reader = await sqlCommand.ExecuteReaderAsync())
        {
            while (reader.Read())
            {
                papers.Add(new Paper
                {
                    Pid = reader.GetInt32("pid"),
                    Pname = reader.GetValue("pname") as string,
                    Psource = reader.GetValue("psource") as string,
                    Pyear = (reader.GetValue("pyear") as DateTime?)?.ToString("yyyy-MM-dd"),
                    Ptype = reader.GetValue("ptype") as int?,
                    Level = reader.GetValue("level") as int?,
                });
            }
        }
        return papers;
    }
    catch (Exception ex)
    {
        Console.WriteLine(ex.Message);
        return null;
    }
}
static async Task<ResultMsg> PostPaper(MySqlConnection sqlConn, Paper paper)
{
    string keys = "pid", values = $"{paper.Pid}";
    if (paper.Pname is not null)
    {
        keys += ",pname";
        values += $",\"{paper.Pname}\"";
    }
    if (paper.Psource is not null)
    {
        keys += ",psource";
        values += $",\"{paper.Psource}\"";
    }
    if (paper.Pyear is not null)
    {
        keys += ",pyear";
        values += $",\"{paper.Pyear}\"";
    }
    if (paper.Ptype is not null)
    {
        keys += ",ptype";
        values += $",{paper.Ptype}";
    }
    if (paper.Level is not null)
    {
        keys += ",level";
        values += $",{paper.Level}";
    }
    var sqlQueryStr = $"insert into paper ({keys}) values ({values})";
    var sqlCommand = new MySqlCommand(sqlQueryStr, sqlConn);
    try
    {
        return new ResultMsg(await sqlCommand.ExecuteNonQueryAsync(), "");
    }
    catch (Exception ex)
    {
        return new ResultMsg(-1, ex.Message);
    }
}
static async Task<ResultMsg> DeletePaper(MySqlConnection sqlConn, int pid)
{
    var para_pid = new MySqlParameter("?pid", MySqlDbType.Int32) { Value = pid, Direction = ParameterDirection.Input };
    var para_out = new MySqlParameter("?state", MySqlDbType.Int32) { Direction = ParameterDirection.Output };
    var sqlCommand = new MySqlCommand { Connection = sqlConn, CommandText = "paper_del", CommandType = CommandType.StoredProcedure };
    sqlCommand.Parameters.Add(para_pid);
    sqlCommand.Parameters.Add(para_out);
    var exec_errno = await sqlCommand.ExecuteNonQueryAsync();
    return new ResultMsg(Convert.ToInt32(para_out.Value) == 0 ? exec_errno : -1, "");

}
static async Task<ResultMsg> PutPaper(MySqlConnection sqlConn, Paper paper)
{
    var sqlQueryStr = "update paper set ";
    var sets = new List<string>();
    var pid = paper.Pid;
    var pname = paper.Pname;
    var psource = paper.Psource;
    var pyear = paper.Pyear;
    var ptype = paper.Ptype;
    var level = paper.Level;
    if (pname is not null) sets.Add($"pname=\"{pname}\"");
    if (psource is not null) sets.Add($"psource=\"{psource}\"");
    if (pyear is not null) sets.Add($"pyear=\"{pyear}\"");
    if (ptype is not null) sets.Add($"ptype={ptype}");
    if (level is not null) sets.Add($"level={level}");
    if (sets.Count > 0)
    {
        sqlQueryStr += string.Join(", ", sets);
        sqlQueryStr += $" where pid={pid}";
        var sqlCommand = new MySqlCommand(sqlQueryStr, sqlConn);
        try
        {
            var res = new ResultMsg(await sqlCommand.ExecuteNonQueryAsync(), "");
            return res;
        }
        catch (Exception ex)
        {
            return new ResultMsg(-1, ex.Message);
        }
    }
    return new ResultMsg(-1, "update set is empty");
}
static async Task<ResultMsg> PostPublishPaper(MySqlConnection sqlConn, int pid, List<PublishPaper> authors)
{
    foreach (var author in authors.Where(author => author.Pid != pid))
        return new ResultMsg(-1, "different pid");
    try
    {
        var sqlQueryStr = $"delete from publish_paper where publish_paper.pid={pid}";
        var sqlCommand = new MySqlCommand(sqlQueryStr, sqlConn);
        await sqlCommand.ExecuteNonQueryAsync();
    }
    catch
    {
        return new ResultMsg(-1, "cannot delete outdated items");
    }

    foreach (var author in authors)
    {
        var para_tid = new MySqlParameter("?tid", MySqlDbType.String) { Value = author.Tid, Direction = ParameterDirection.Input };
        var para_pid = new MySqlParameter("?pid", MySqlDbType.Int32) { Value = pid, Direction = ParameterDirection.Input };
        var para_ptrank = new MySqlParameter("?ptrank", MySqlDbType.Int32) { Value = author.Ptrank, Direction = ParameterDirection.Input };
        var para_correspond = new MySqlParameter("?correspond", MySqlDbType.Int32) { Value = author.Correspond, Direction = ParameterDirection.Input };
        var para_out = new MySqlParameter("?state", MySqlDbType.Int32) { Direction = ParameterDirection.Output };
        var sqlCommand = new MySqlCommand { Connection = sqlConn, CommandText = "publish_paper_insert", CommandType = CommandType.StoredProcedure };
        sqlCommand.Parameters.Add(para_tid);
        sqlCommand.Parameters.Add(para_pid);
        sqlCommand.Parameters.Add(para_ptrank);
        sqlCommand.Parameters.Add(para_correspond);
        sqlCommand.Parameters.Add(para_out);
        var exec_res = await sqlCommand.ExecuteNonQueryAsync();
        if (Convert.ToInt32(para_out.Value) == 0 && exec_res == 1) continue;
        return new ResultMsg(-1, $"cannot insert author {author.Tid}");
    }
    return new ResultMsg(0, "");
}
static async Task<List<PublishPaper>?> GetPublishPaper(
    MySqlConnection sqlConn,
    int? pid = null, string? tid = null)
{
    var sqlQueryStr = "Select * from publish_paper";

    var wheres = new List<string>();
    if (pid != null) wheres.Add($"pid={pid}");
    if (tid != null) wheres.Add($"tid='{tid}'");
    if (wheres.Count > 0)
    {
        sqlQueryStr += " where ";
        sqlQueryStr += string.Join(" and ", wheres);
    }
    try
    {
        var sqlCommand = new MySqlCommand(sqlQueryStr, sqlConn);
        var publish_papers = new List<PublishPaper>();
        using var reader = await sqlCommand.ExecuteReaderAsync();
        while (reader.Read())
        {
            publish_papers.Add(new PublishPaper
            {
                Tid = reader.GetString("tid"),
                Pid = reader.GetInt32("pid"),
                Ptrank = reader.GetValue("ptrank") as int?,
                Correspond = reader.GetValue("correspond") as bool?
            });
        }

        return publish_papers;
    }
    catch (Exception ex)
    {
        Console.WriteLine(ex.Message);
        return null;
    }
}

app.MapPost("/paper", async ([FromBody] Paper paper) =>
{
    var sqlConn = new MySqlConnection(sqlConnCommand);
    try
    {
        await sqlConn.OpenAsync();
    }
    catch
    {
        return Results.BadRequest(new ResultMsg(-1, "database connection failed"));
    }
    var res = await PostPaper(sqlConn, paper);
    return res.Result >= 0 ? Results.Ok(res) : Results.BadRequest(res);
});
app.MapDelete("/paper", async (int pid) =>
{
    var sqlConn = new MySqlConnection(sqlConnCommand);
    try
    {
        await sqlConn.OpenAsync();
    }
    catch
    {
        return Results.BadRequest(new ResultMsg(-1, "database connection failed"));
    }
    var res = await DeletePaper(sqlConn, pid);
    return res.Result >= 0 ? Results.Ok(res) : Results.BadRequest(res);
});
app.MapPut("/paper", async ([FromBody] Paper paper) =>
{
    var sqlConn = new MySqlConnection(sqlConnCommand);
    try
    {
        await sqlConn.OpenAsync();
    }
    catch
    {
        return Results.BadRequest(new ResultMsg(-1, "database connection failed"));
    }
    var res = await PutPaper(sqlConn, paper);
    return res.Result >= 0 ? Results.Ok(res) : Results.BadRequest(res);
});
app.MapGet("/paper", async (
    int? pid, string? pname, string? psource, DateTime? pyear_from, DateTime? pyear_to, int? ptype, int? level,
    string? orderby, bool? desc, int? limit) =>
{
    var sqlConn = new MySqlConnection(sqlConnCommand);
    try
    {
        await sqlConn.OpenAsync();
    }
    catch
    {
        return Results.NotFound();
    }
    var papers = await GetPaper(sqlConn, pid, pname, psource, pyear_from, pyear_to, ptype, level, orderby, desc, limit);

    return papers is null ? Results.NotFound() : Results.Ok(papers);
});

app.MapGet("/publish-paper/{pid}", async ([FromRoute] int pid) =>
{
    var sqlConn = new MySqlConnection(sqlConnCommand);
    await sqlConn.OpenAsync();
    using var sqlTrans = sqlConn.BeginTransaction();
    var papers = await GetPaper(sqlConn, pid);
    if (papers is null) return Results.NotFound();

    if (papers.Count != 1) return Results.BadRequest();

    var publish_papers = await GetPublishPaper(sqlConn, pid: pid);
    if (publish_papers is null)
    {
        await sqlTrans.RollbackAsync();
        return Results.NotFound();
    }
    return Results.Ok(new PaperDetail(papers[0], publish_papers));
});
app.MapPost("/publish-paper/{pid}", async ([FromRoute] int pid, [FromBody] List<PublishPaper> authors) =>
{
    var sqlConn = new MySqlConnection(sqlConnCommand);
    await sqlConn.OpenAsync();
    using var sqlTrans = sqlConn.BeginTransaction();
    var res = await PostPublishPaper(sqlConn, pid, authors);
    if (res.Result < 0)
    {
        await sqlTrans.RollbackAsync();
        return Results.BadRequest(res);
    }

    await sqlTrans.CommitAsync();
    return Results.Ok();
});
/*
app.MapPost("/paper/detail/{pid}", async ([FromRoute] int pid, [FromBody] PaperDetail paper_detail) =>
{
    var sqlConn = new MySqlConnection(sqlConnCommand);
    await sqlConn.OpenAsync();
    using var sqlTrans = sqlConn.BeginTransaction();

    var paper = paper_detail.Paper;
    var paper_res = await PutPaper(sqlConn, paper);
    if (paper_res.Result < 0)
    {
        await sqlTrans.RollbackAsync();
        return Results.BadRequest(paper_res);
    }

    var authors = paper_detail.Authors;
    var res = await PostPublishPaper(sqlConn, pid, authors);
    if (res.Result < 0)
    {
        await sqlTrans.RollbackAsync();
        return Results.BadRequest(res);
    }

    await sqlTrans.CommitAsync();
    return Results.Ok();
});
*/
#endregion

#region project
static async Task<List<Project>?> GetProject(
    MySqlConnection sqlConn,
    string? jid = null, string? jname = null, string? jsource = null, int? jtype = null, float? jbudgets = null,
    DateTime? styear = null, DateTime? edyear = null,
    string? orderby = null, bool? desc = null, int? limit = null)
{
    limit ??= 30;
    var sqlQueryStr = "Select * from project";

    var wheres = new List<string>();
    if (jid != null) wheres.Add($"jid='{jid}'");
    if (jname != null) wheres.Add($"jname like '%{jname}%'");
    if (jsource != null) wheres.Add($"jsource like '%{jsource}%'");
    if (jtype != null) wheres.Add($"jtype={jtype}");
    if (jbudgets != null) wheres.Add($"jbudgets={jbudgets}");
    if (styear != null) wheres.Add($"styear>='{styear}'");
    if (edyear != null) wheres.Add($"edyear<='{edyear}'");
    if (wheres.Count > 0)
    {
        sqlQueryStr += " where ";
        sqlQueryStr += string.Join(" and ", wheres);
    }
    sqlQueryStr += $" order by {orderby ?? "jid"}";
    if (desc ?? false) sqlQueryStr += " desc";
    sqlQueryStr += $" limit {Math.Max(Math.Min(limit ?? 0, MaxSqlResultLength), 0)}";

    try
    {
        var sqlCommand = new MySqlCommand(sqlQueryStr, sqlConn);
        var projects = new List<Project>();
        using (var reader = await sqlCommand.ExecuteReaderAsync())
        {
            while (reader.Read())
            {
                projects.Add(new Project
                {
                    Jid = reader.GetString("jid"),
                    Jname = reader.GetValue("jname") as string,
                    Jsource = reader.GetValue("jsource") as string,
                    Jtype = reader.GetValue("jtype") as int?,
                    Jbudgets = reader.GetValue("jbudgets") as float?,
                    Styear = reader.GetValue("styear") as int?,
                    Edyear = reader.GetValue("edyear") as int?
                });
            }
        }
        return projects;
    }
    catch (Exception ex)
    {
        Console.WriteLine(ex.Message);
        return null;
    }
}
static async Task<ResultMsg> PostProject(MySqlConnection sqlConn, Project project)
{
    if (project.Jbudgets != 0) return new ResultMsg(-1, "jbudgets must be 0 when post");
    string keys = "jid", values = $"{project.Jid}";
    if (project.Jname != null) { keys += ",jname"; values += $",'{project.Jname}'"; }
    if (project.Jsource != null) { keys += ",jsource"; values += $",'{project.Jsource}'"; }
    if (project.Jtype != null) { keys += ",jtype"; values += $",{project.Jtype}"; }
    if (project.Jbudgets != null) { keys += ",jbudgets"; values += $",{project.Jbudgets}"; }
    if (project.Styear != null) { keys += ",styear"; values += $",{project.Styear}"; }
    if (project.Edyear != null) { keys += ",edyear"; values += $",{project.Edyear}"; }
    var sqlQueryStr = $"insert into project ({keys}) values ({values})";
    var sqlCommand = new MySqlCommand(sqlQueryStr, sqlConn);
    try
    {
        return new ResultMsg(await sqlCommand.ExecuteNonQueryAsync(), "");
    }
    catch (Exception ex)
    {
        return new ResultMsg(-1, ex.Message);
    }
}
static async Task<ResultMsg> DeleteProject(MySqlConnection sqlConn, int jid)
{
    var para_jid = new MySqlParameter("?jid", MySqlDbType.Int32) { Value = jid, Direction = ParameterDirection.Input };
    var para_out = new MySqlParameter("?state", MySqlDbType.Int32) { Direction = ParameterDirection.Output };
    var sqlCommand = new MySqlCommand { Connection = sqlConn, CommandText = "project_del", CommandType = CommandType.StoredProcedure };
    sqlCommand.Parameters.Add(para_jid);
    sqlCommand.Parameters.Add(para_out);
    var exec_errno = await sqlCommand.ExecuteNonQueryAsync();
    return new ResultMsg(Convert.ToInt32(para_out.Value) == 0 ? exec_errno : -1, "");

}
static async Task<ResultMsg> PutProject(MySqlConnection sqlConn, Project project, bool with_check = true)
{
    var sqlQueryStr = "update project set ";
    var sets = new List<string>();
    var jid = project.Jid;
    if (jid is null) return new ResultMsg(-1, "jid must not be null");
    if (with_check)
    {
        var preSqlQueryStr = $"select SUM(jtbudget) as sum from project_undertaken where jid='{jid}'";
        var preSqlCommand = new MySqlCommand(preSqlQueryStr, sqlConn);
        var jbudget_sum = Convert.ToDouble(await preSqlCommand.ExecuteScalarAsync());
        if (Math.Abs(jbudget_sum - project.Jbudgets ?? 0) > 1e-4)
            return new ResultMsg(-1, "jbudgets must be equal to the sum of jtbudget in project_undertaken");
    }

    var jname = project.Jname;
    var jsource = project.Jsource;
    var jtype = project.Jtype;
    var jbudgets = project.Jbudgets;
    var styear = project.Styear;
    var edyear = project.Edyear;
    if (jname is not null) sets.Add($"jname=\"{jname}\"");
    if (jsource is not null) sets.Add($"jsource=\"{jsource}\"");
    if (jtype is not null) sets.Add($"jtype={jtype}");
    if (jbudgets is not null) sets.Add($"jbudgets={jbudgets}");
    if (styear is not null) sets.Add($"styear={styear}");
    if (edyear is not null) sets.Add($"edyear={edyear}");
    if (sets.Count > 0)
    {
        sqlQueryStr += string.Join(", ", sets);
        sqlQueryStr += $" where jid='{jid}'";
        var sqlCommand = new MySqlCommand(sqlQueryStr, sqlConn);
        try
        {
            var res = new ResultMsg(await sqlCommand.ExecuteNonQueryAsync(), "");
            return res;
        }
        catch (Exception ex)
        {
            return new ResultMsg(-1, ex.Message);
        }
    }
    return new ResultMsg(-1, "update set is empty");
}
static async Task<ResultMsg> PostProjectUndertaken(MySqlConnection sqlConn, string jid, List<ProjectUndertaken> managers, bool with_check = true)
{
    foreach (var manager in managers.Where(manager => manager.Jid != jid))
        return new ResultMsg(-1, "different jid");
    if (with_check)
    {
        float jbudget_sum = 0;
        foreach (var manager in managers)
            jbudget_sum += manager.Jtbudget ?? 0;

        var preSqlQueryStr = $"Select jbudgets from project where jid='{jid}'";
        var preSqlCommand = new MySqlCommand(preSqlQueryStr, sqlConn);
        var jbudgets = Convert.ToDouble(await preSqlCommand.ExecuteScalarAsync());
        if (Math.Abs(jbudgets - jbudget_sum) > 1e-4)
            return new ResultMsg(-1, "the sum of jtbudget must be equal to jbudgets in project");
    }
    try
    {
        var sqlQueryStr = $"delete from project_undertaken where project_undertaken.jid='{jid}'";
        var sqlCommand = new MySqlCommand(sqlQueryStr, sqlConn);
        await sqlCommand.ExecuteNonQueryAsync();
    }
    catch
    {
        return new ResultMsg(-1, "cannot delete outdated items");
    }

    foreach (var manager in managers)
    {
        var para_tid = new MySqlParameter("?tid", MySqlDbType.String) { Value = manager.Tid, Direction = ParameterDirection.Input };
        var para_jid = new MySqlParameter("?jid", MySqlDbType.String) { Value = jid, Direction = ParameterDirection.Input };
        var para_jtrank = new MySqlParameter("?jtrank", MySqlDbType.Int32) { Value = manager.Jtrank, Direction = ParameterDirection.Input };
        var para_jtbudget = new MySqlParameter("?jtbudget", MySqlDbType.Float) { Value = manager.Jtbudget, Direction = ParameterDirection.Input };
        var para_out = new MySqlParameter("?state", MySqlDbType.Int32) { Direction = ParameterDirection.Output };
        var sqlCommand = new MySqlCommand { Connection = sqlConn, CommandText = "project_undertaken_insert", CommandType = CommandType.StoredProcedure };
        sqlCommand.Parameters.Add(para_tid);
        sqlCommand.Parameters.Add(para_jid);
        sqlCommand.Parameters.Add(para_jtrank);
        sqlCommand.Parameters.Add(para_jtbudget);
        sqlCommand.Parameters.Add(para_out);
        var exec_res = await sqlCommand.ExecuteNonQueryAsync();
        if (Convert.ToInt32(para_out.Value) == 0 && exec_res == 1) continue;
        return new ResultMsg(-1, $"cannot insert manager {manager.Tid}");
    }
    return new ResultMsg(0, "");
}
static async Task<List<ProjectUndertaken>?> GetProjectUndertaken(
    MySqlConnection sqlConn,
    string? jid = null, string? tid = null)
{
    var sqlQueryStr = "Select * from project_undertaken";

    var wheres = new List<string>();
    if (jid != null) wheres.Add($"jid='{jid}'");
    if (tid != null) wheres.Add($"tid='{tid}'");
    if (wheres.Count > 0)
    {
        sqlQueryStr += " where ";
        sqlQueryStr += string.Join(" and ", wheres);
    }
    try
    {
        var sqlCommand = new MySqlCommand(sqlQueryStr, sqlConn);

        var project_undertaken = new List<ProjectUndertaken>();
        using var reader = await sqlCommand.ExecuteReaderAsync();
        while (reader.Read())
        {
            project_undertaken.Add(new ProjectUndertaken
            {
                Tid = reader.GetString("tid"),
                Jid = reader.GetString("jid"),
                Jtrank = reader.GetInt32("jtrank"),
                Jtbudget = reader.GetFloat("jtbudget")
            });
        }
        await reader.CloseAsync();
        return project_undertaken;
    }
    catch (Exception ex)
    {
        Console.WriteLine(ex.Message);
        return null;
    }
}

app.MapPost("/project", async ([FromBody] Project project) =>
{
    var sqlConn = new MySqlConnection(sqlConnCommand);
    try
    {
        await sqlConn.OpenAsync();
    }
    catch
    {
        return Results.BadRequest(new ResultMsg(-1, "database connection failed"));
    }
    var res = await PostProject(sqlConn, project);
    return res.Result >= 0 ? Results.Ok(res) : Results.BadRequest(res);
});
app.MapDelete("/project", async (int jid) =>
{
    var sqlConn = new MySqlConnection(sqlConnCommand);
    try
    {
        await sqlConn.OpenAsync();
    }
    catch
    {
        return Results.BadRequest(new ResultMsg(-1, "database connection failed"));
    }
    var res = await DeleteProject(sqlConn, jid);
    return res.Result >= 0 ? Results.Ok(res) : Results.BadRequest(res);
});
app.MapPut("/project", async ([FromBody] Project project) =>
{
    var sqlConn = new MySqlConnection(sqlConnCommand);
    try
    {
        await sqlConn.OpenAsync();
    }
    catch
    {
        return Results.BadRequest(new ResultMsg(-1, "database connection failed"));
    }
    var res = await PutProject(sqlConn, project);
    return res.Result >= 0 ? Results.Ok(res) : Results.BadRequest(res);
});
app.MapGet("/project", async (
    string? jid, string? jname, string? jsource, int? jtype, int? jbudgets, DateTime? styear, DateTime? edyear,
    string? orderby, bool? desc, int? limit) =>
{
    var sqlConn = new MySqlConnection(sqlConnCommand);
    try
    {
        await sqlConn.OpenAsync();
    }
    catch
    {
        return Results.NotFound();
    }
    var projects = await GetProject(sqlConn, jid, jname, jsource, jtype, jbudgets, styear, edyear, orderby, desc, limit);

    return projects is null ? Results.NotFound() : Results.Ok(projects);
});

app.MapGet("/project-undertaken/{jid}", async ([FromRoute] string jid) =>
{
    var sqlConn = new MySqlConnection(sqlConnCommand);
    await sqlConn.OpenAsync();
    using var sqlTrans = sqlConn.BeginTransaction();
    var projects = await GetProject(sqlConn, jid);
    if (projects is null) return Results.NotFound();
    if (projects.Count != 1) return Results.BadRequest();

    var sqlQueryStr = $"Select * from project_undertaken where jid='{jid}'";
    var sqlCommand = new MySqlCommand(sqlQueryStr, sqlConn);

    var project_undertaken = await GetProjectUndertaken(sqlConn, jid: jid);
    if (project_undertaken is null)
    {
        await sqlTrans.RollbackAsync();
        return Results.NotFound();
    }
    await sqlTrans.CommitAsync();
    return Results.Ok(new ProjectDetail(projects[0], project_undertaken));
});
app.MapPost("/project-undertaken/{jid}", async ([FromRoute] string jid, [FromBody] List<ProjectUndertaken> managers) =>
{
    var sqlConn = new MySqlConnection(sqlConnCommand);
    await sqlConn.OpenAsync();
    using var sqlTrans = sqlConn.BeginTransaction();
    var res = await PostProjectUndertaken(sqlConn, jid, managers);
    if (res.Result < 0)
    {
        await sqlTrans.RollbackAsync();
        return Results.BadRequest(res);
    }

    await sqlTrans.CommitAsync();
    return Results.Ok();
});

app.MapPost("/project/detail/{jid}", async ([FromRoute] string jid, [FromBody] ProjectDetail project_detail) =>
{
    var project = project_detail.Project;
    var managers = project_detail.Managers;
    float jbudget_sum = 0;
    foreach (var manager in managers)
        jbudget_sum += manager.Jtbudget ?? 0;
    if (Math.Abs(jbudget_sum - project.Jbudgets ?? 0) > 1e-4)
        return Results.BadRequest(new ResultMsg(-1, "jbudgets of project must be equal to the sum of jtbudget in managers"));

    var sqlConn = new MySqlConnection(sqlConnCommand);
    await sqlConn.OpenAsync();
    using var sqlTrans = sqlConn.BeginTransaction();

    var project_res = await PutProject(sqlConn, project, with_check:false);
    if (project_res.Result < 0)
    {
        await sqlTrans.RollbackAsync();
        return Results.BadRequest(project_res);
    }

    var res = await PostProjectUndertaken(sqlConn, jid, managers, with_check:false);
    if (res.Result < 0)
    {
        await sqlTrans.RollbackAsync();
        return Results.BadRequest(res);
    }

    await sqlTrans.CommitAsync();
    return Results.Ok();
});

#endregion

#region course
static async Task<List<Course>?> GetCourse(
    MySqlConnection sqlConn,
    string? cid = null, string? cname = null, int? chour = null, int? cnature = null,
    string? orderby = null, bool? desc = null, int? limit = null)
{
    limit ??= 30;
    var sqlQueryStr = "Select * from course";

    var wheres = new List<string>();
    if (cid != null) wheres.Add($"cid='{cid}'");
    if (cname != null) wheres.Add($"cname like '%{cname}%'");
    if (chour != null) wheres.Add($"chour={chour}");
    if (cnature != null) wheres.Add($"cnature={cnature}");
    if (wheres.Count > 0)
    {
        sqlQueryStr += " where ";
        sqlQueryStr += string.Join(" and ", wheres);
    }
    sqlQueryStr += $" order by {orderby ?? "cid"}";
    if (desc ?? false) sqlQueryStr += " desc";
    sqlQueryStr += $" limit {Math.Max(Math.Min(limit ?? 0, MaxSqlResultLength), 0)}";

    try
    {
        var sqlCommand = new MySqlCommand(sqlQueryStr, sqlConn);
        var courses = new List<Course>();
        using (var reader = await sqlCommand.ExecuteReaderAsync())
        {
            while (reader.Read())
            {
                courses.Add(new Course
                {
                    Cid = reader.GetString("cid"),
                    Cname = reader.GetValue("cname") as string,
                    Chour = reader.GetValue("chour") as int?,
                    Cnature = reader.GetValue("cnature") as int?
                });
            }
        }
        return courses;
    }
    catch (Exception ex)
    {
        Console.WriteLine(ex.Message);
        return null;
    }

}
static async Task<List<CourseTaught>?> GetCourseTaught(
    MySqlConnection sqlConn,
    string? cid = null, string? tid = null)
{
    var sqlQueryStr = "Select * from course_taught";

    var wheres = new List<string>();
    if (cid != null) wheres.Add($"cid='{cid}'");
    if (tid != null) wheres.Add($"tid='{tid}'");
    if (wheres.Count > 0)
    {
        sqlQueryStr += " where ";
        sqlQueryStr += string.Join(" and ", wheres);
    }
    try
    {
        var sqlCommand = new MySqlCommand(sqlQueryStr, sqlConn);
        var course_taught = new List<CourseTaught>();
        using var reader = await sqlCommand.ExecuteReaderAsync();
        while (reader.Read())
        {
            course_taught.Add(new CourseTaught
            {
                Tid = reader.GetString("tid"),
                Cid = reader.GetString("cid"),
                Tyear = reader.GetInt32("tyear"),
                Tterm = reader.GetInt32("tterm"),
                Thour = reader.GetInt32("thour")
            });
        }
        await reader.CloseAsync();
        return course_taught;
    }
    catch (Exception ex)
    {
        Console.WriteLine(ex.Message);
        return null;
    }

}

app.MapGet("/course", async (
    string? cid, string? cname, int? chour, int? cnature,
    string? orderby, bool? desc, int? limit) =>
{
    var sqlConn = new MySqlConnection(sqlConnCommand);
    try
    {
        await sqlConn.OpenAsync();
    }
    catch
    {
        return Results.NotFound();
    }
    var courses = await GetCourse(sqlConn, cid, cname, chour, cnature, orderby, desc, limit);

    return courses is null ? Results.NotFound() : Results.Ok(courses);
});
app.MapGet("/course-taught/{cid}", async ([FromRoute] string cid) =>
{
    var sqlConn = new MySqlConnection(sqlConnCommand);
    await sqlConn.OpenAsync();
    using var sqlTrans = sqlConn.BeginTransaction();
    var courses = await GetCourse(sqlConn, cid);
    if (courses is null) return Results.NotFound();
    if (courses.Count != 1) return Results.BadRequest();

    var course_taught = await GetCourseTaught(sqlConn, cid: cid);
    if (course_taught is null)
    {
        await sqlTrans.RollbackAsync();
        return Results.NotFound();
    }
    await sqlTrans.CommitAsync();
    return Results.Ok(new CourseDetail(courses[0], course_taught));
});

#endregion

#region teacher
static async Task<List<Teacher>?> GetTeacher(
    MySqlConnection sqlConn,
    string? tid = null, string? tname = null, int? gender = null, int? title = null,
    string? orderby = null, bool? desc = null, int? limit = null)
{
    limit ??= 30;
    var sqlQueryStr = "Select * from teacher";

    var wheres = new List<string>();
    if (tid != null) wheres.Add($"tid='{tid}'");
    if (tname != null) wheres.Add($"tname='{tname}'");
    if (gender != null) wheres.Add($"gender={gender}");
    if (title != null) wheres.Add($"title={title}");
    if (wheres.Count > 0)
    {
        sqlQueryStr += " where ";
        sqlQueryStr += string.Join(" and ", wheres);
    }
    sqlQueryStr += $" order by {orderby ?? "tid"}";
    if (desc ?? false) sqlQueryStr += " desc";
    sqlQueryStr += $" limit {Math.Max(Math.Min(limit ?? 0, MaxSqlResultLength), 0)}";

    try
    {
        var sqlCommand = new MySqlCommand(sqlQueryStr, sqlConn);
        var teachers = new List<Teacher>();
        using (var reader = await sqlCommand.ExecuteReaderAsync())
        {
            while (reader.Read())
            {
                teachers.Add(new Teacher
                {
                    Tid = reader.GetString("tid"),
                    Tname = reader.GetValue("tname") as string,
                    Gender = reader.GetValue("gender") as int?,
                    Title = reader.GetValue("title") as int?
                });
            }
        }
        return teachers;
    }
    catch (Exception ex)
    {
        Console.WriteLine(ex.Message);
        return null;
    }

}

app.MapGet("/teacher", async (
    string? tid, string? tname, int? gender, int? title,
    string? orderby, bool? desc, int? limit) =>
{
    var sqlConn = new MySqlConnection(sqlConnCommand);
    try
    {
        await sqlConn.OpenAsync();
    }
    catch
    {
        return Results.NotFound();
    }
    var teachers = await GetTeacher(sqlConn, tid, tname, gender, title, orderby, desc, limit);

    return teachers is null ? Results.NotFound() : Results.Ok(teachers);
});

app.MapGet("/teacher/detail/{tid}", async ([FromRoute] string tid) =>
{
    var sqlConn = new MySqlConnection(sqlConnCommand);
    await sqlConn.OpenAsync();
    using var sqlTrans = sqlConn.BeginTransaction();
    var teacher = await GetTeacher(sqlConn, tid: tid);
    if (teacher is null)
    {
        await sqlTrans.RollbackAsync();
        return Results.NotFound();
    }
    if (teacher.Count != 1) return Results.BadRequest();

    var courses = await GetCourseTaught(sqlConn, tid: tid);
    if (courses is null)
    {
        await sqlTrans.RollbackAsync();
        return Results.NotFound();
    }
    var papers = await GetPublishPaper(sqlConn, tid: tid);
    if (papers is null)
    {
        await sqlTrans.RollbackAsync();
        return Results.NotFound();
    }
    var projects = await GetProjectUndertaken(sqlConn, tid: tid);
    if (projects is null)
    {
        await sqlTrans.RollbackAsync();
        return Results.NotFound();
    }

    await sqlTrans.CommitAsync();
    return Results.Ok(new TeacherDetail(teacher[0], courses: courses, papers: papers, projects: projects));
});

#endregion

app.Run();

record ResultMsg(int Result, string Msg);
