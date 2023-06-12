using Microsoft.AspNetCore.Mvc;
using MySql.Data.MySqlClient;
using Newtonsoft.Json.Linq;
using System.Data;
using WebApi.Tables;

namespace WebApi.Controllers
{
    [ApiController]
    [Route("paper/insert")]
    public class PaperInsertController : ControllerBase
    {
        private readonly ILogger<PaperInsertController> _logger;

        public PaperInsertController(ILogger<PaperInsertController> logger)
        {
            _logger = logger;
        }

        [HttpPost(Name = "PaperInsert")]
        public JObject Post(int? pid, string? pname, string? psource, DateTime? pyear, int? ptype, int? level)
        {
            var res = new JObject
            {
                { "result", false },
                { "msg", "" }
            };

            if (pid is null)
            {
                res["msg"] = "pid is empty";
                return res;
            }
            var sqlQueryStr = "insert into paper (pid, pname, psource, pyear, ptype, level) values(";
            sqlQueryStr += pid is null ? "null," : $"{pid},";
            sqlQueryStr += pname is null ? "null," : $"\"{pname}\",";
            sqlQueryStr += psource is null ? "null," : $"\"{psource}\",";
            sqlQueryStr += pyear is null ? "null," : $"\"{pyear?.ToString("yyyy-MM-dd")}\",";
            sqlQueryStr += ptype is null ? "null," : $"{ptype},";
            sqlQueryStr += level is null ? "null" : $"{level}";
            sqlQueryStr += ")";
            Console.WriteLine(sqlQueryStr);
            var sqlCommand = new MySqlCommand(sqlQueryStr, Program.connection);
            try
            {
                res["result"] = Convert.ToBoolean(sqlCommand.ExecuteNonQuery());
            }
            catch(Exception ex)
            {
                res["msg"] = ex.Message;
            }
            return res;
        }
    }

    [ApiController]
    [Route("paper/delete")]
    public class PaperDeleteController : ControllerBase
    {
        private readonly ILogger<PaperDeleteController> _logger;

        public PaperDeleteController(ILogger<PaperDeleteController> logger)
        {
            _logger = logger;
        }

        [HttpPost(Name = "PaperDelete")]
        public JObject Post(int? pid)
        {
            var res = new JObject
            {
                { "result", false },
                { "msg", "" }
            };

            if (pid is null)
            {
                res["msg"] = "pid is empty";
                return res;
            }
            var command = new MySqlCommand();
            command.Connection = Program.connection;
            command.CommandText = "paper_del";
            command.CommandType = CommandType.StoredProcedure;
            MySqlParameter para_pid = new MySqlParameter("?pid", MySqlDbType.Int32);
            MySqlParameter para_out = new MySqlParameter("?state", MySqlDbType.Int32);
            para_pid.Value = pid;
            para_pid.Direction = ParameterDirection.Input;
            para_out.Direction = ParameterDirection.Output;
            command.Parameters.Add(para_pid);
            command.Parameters.Add(para_out);
            var exec_errno = command.ExecuteNonQuery();
            Console.WriteLine(para_out.Value);
            res["result"] = Convert.ToInt32(para_out.Value);
            return res;
        }
    }

    [ApiController]
    [Route("paper/update")]
    public class PaperUpdateController : ControllerBase
    {
        private readonly ILogger<PaperUpdateController> _logger;

        public PaperUpdateController(ILogger<PaperUpdateController> logger)
        {
            _logger = logger;
        }

        [HttpPost(Name = "PaperUpdate")]
        public JObject Post(int? pid, string? pname, string? psource, DateTime? pyear, int? ptype, int? level)
        {
            var res = new JObject
            {
                { "result", false },
                { "msg", "" }
            };

            if (pid is null)
            {
                res["msg"] = "pid is empty";
                return res;
            }
            var sqlQueryStr = "update paper set ";
            var sets = new List<string>();
            if (pname is not null) sets.Add($"pname=\"{pname}\"");
            if (psource is not null) sets.Add($"psource=\"{psource}\"");
            if (pyear is not null) sets.Add($"pyear=\"{pyear?.ToString("yyyy-MM-dd")}\"");
            if (ptype is not null) sets.Add($"ptype={ptype}");
            if (level is not null) sets.Add($"level={level}");
            if (sets.Count > 0)
            {
                sqlQueryStr += string.Join(", ", sets);
                sqlQueryStr += $" where pid={pid}";
                var sqlCommand = new MySqlCommand(sqlQueryStr, Program.connection);
                res["result"] = Convert.ToBoolean(sqlCommand.ExecuteNonQuery());
            }
            else
            {
                res["msg"] = "update set is empty";
            }
            return res;
        }
    }
    [ApiController]
    [Route("paper/get")]
    public class PaperGetController : ControllerBase
    {
        private readonly ILogger<PaperGetController> _logger;

        public PaperGetController(ILogger<PaperGetController> logger)
        {
            _logger = logger;
        }

        [HttpGet(Name = "PaperGet")]
        public IEnumerable<Paper> Get(int? pid, string? pname, string? psource, DateTime? pyear, int? ptype, int? level, string? orderby, bool? desc, int? limit)
        {
            limit ??= 30;
            var sqlQueryStr = "Select * from paper";

            var wheres = new List<string>();
            if (pid is not null) wheres.Add($"pid={pid}");
            if (pname is not null) wheres.Add($"pname=\"{pname}\"");
            if (psource is not null) wheres.Add($"psource=\"{psource}\"");
            if (pyear is not null) wheres.Add($"pyear=\"{pyear}\"");
            if (ptype is not null) wheres.Add($"ptype={ptype}");
            if (level is not null) wheres.Add($"level={level}");
            if (wheres.Count > 0)
            {
                sqlQueryStr += " where ";
                sqlQueryStr += string.Join(" and ", wheres);
            }
            sqlQueryStr += $" Order by {orderby ?? "pid"}";
            if (desc ?? false) sqlQueryStr += " desc";
            sqlQueryStr += $" limit {Math.Max(Math.Min(limit ?? 0, Program.MaxSqlResultLength), 0)}";

            var sqlCommand = new MySqlCommand(sqlQueryStr, Program.connection);
            var papers = new List<Paper>();
            var reader = sqlCommand.ExecuteReader();
            if (reader is not null)
            {
                while (reader.Read())
                {
                    papers.Add(new Paper
                    {
                        Pid = reader.GetInt32("pid"),
                        Pname = reader.GetString("pname"),
                        Psource = reader.GetString("psource"),
                        Pyear = reader.GetDateTime("pyear"),
                        Ptype = reader.GetInt32("ptype"),
                        Level = reader.GetInt32("level"),
                    });
                    Console.WriteLine(papers);
                }
            }

            reader?.Close();
            return papers;
        }
    }
}