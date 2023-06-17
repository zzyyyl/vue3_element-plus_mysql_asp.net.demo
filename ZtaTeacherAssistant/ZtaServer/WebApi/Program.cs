using Microsoft.AspNetCore.Mvc;
using MySql.Data.MySqlClient;
using Newtonsoft.Json.Linq;
using System.Data;
using WebApi.Tables;
using System.Linq;

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

    var sqlQueryStr = $"Select * from publish_paper where pid={pid}";
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
    await reader.CloseAsync();
    await sqlTrans.CommitAsync();
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

#region teacher
app.MapPost("/teacher", async (string tid, string? tname, int? gender, int? title) =>
{
    string keys = "tid", values = $"\"{tid}\"";
    if (tname is not null)
    {
        keys += ",tname";
        values += $",\"{tname}\"";
    }
    if (gender is not null)
    {
        keys += ",gender";
        values += $",{gender}";
    }
    if (title is not null)
    {
        keys += ",title";
        values += $",{title}";
    }
    var sqlQueryStr = $"insert into teacher ({keys}) values ({values})";
    var sqlConn = new MySqlConnection(sqlConnCommand);
    await sqlConn.OpenAsync();
    var sqlCommand = new MySqlCommand(sqlQueryStr, sqlConn);
    try
    {
        var res = new ResultMsg(await sqlCommand.ExecuteNonQueryAsync(), "");
        return res.Result == 1 ? Results.Ok(res) : Results.BadRequest(res);
    }
    catch (Exception ex)
    {
        return Results.BadRequest(new ResultMsg(-1, ex.Message));
    }
});
#endregion


app.Run();

record ResultMsg(int Result, string Msg);
