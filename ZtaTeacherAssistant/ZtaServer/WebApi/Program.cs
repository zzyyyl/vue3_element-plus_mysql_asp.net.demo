using Microsoft.AspNetCore.Mvc;
using MySql.Data.MySqlClient;
using Newtonsoft.Json.Linq;
using System.Data;
using WebApi.Tables;

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
app.MapPost("/paper", async ([FromBody]Paper paper) =>
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
app.MapDelete("/paper", async (int pid) =>
{
    var para_pid = new MySqlParameter("?pid", MySqlDbType.Int32) { Value = pid, Direction = ParameterDirection.Input };
    var para_out = new MySqlParameter("?state", MySqlDbType.Int32) { Direction = ParameterDirection.Output };
    var sqlConn = new MySqlConnection(sqlConnCommand);
    await sqlConn.OpenAsync();
    var sqlCommand = new MySqlCommand { Connection = sqlConn, CommandText = "paper_del", CommandType = CommandType.StoredProcedure };
    sqlCommand.Parameters.Add(para_pid);
    sqlCommand.Parameters.Add(para_out);
    var exec_errno = await sqlCommand.ExecuteNonQueryAsync();
    var res = new ResultMsg(Convert.ToInt32(para_out.Value), "");
    return res.Result == 1 ? Results.Ok(res) : Results.BadRequest(res);
});
app.MapPut("/paper", async ([FromBody] Paper paper) =>
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
    }
    return Results.BadRequest(new ResultMsg(-1, "update set is empty"));
});
app.MapGet("/paper", async (
    int? pid, string? pname, string? psource, DateTime? pyear, int? ptype, int? level,
    string? orderby, bool? desc, int? limit) =>
{
    limit ??= 30;
    var sqlQueryStr = "Select * from paper";

    var wheres = new List<string>();
    if (pid is not null) wheres.Add($"pid={pid}");
    if (pname is not null) wheres.Add($"pname='{pname}'");
    if (psource is not null) wheres.Add($"psource='{psource}'");
    if (pyear is not null) wheres.Add($"pyear='{pyear}'");
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

    var sqlConn = new MySqlConnection(sqlConnCommand);
    await sqlConn.OpenAsync();
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

    return Results.Ok(papers);
});
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
