﻿using DSkin.DirectUI;
using FreeSqlTools.Models;
using RazorEngine;
using RazorEngine.Templating;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace FreeSqlTools.Pages
{
    public class CodeGenerate : DSkin.Forms.MiniBlinkPage
    {


        string path = Environment.CurrentDirectory + "\\generate";

        [JSFunction]
        public string GenerateCode()
        {
            var strjson = File.ReadAllText(Environment.CurrentDirectory + "\\demo.json");
            try
            {
                using (IFreeSql fsql = new FreeSql.FreeSqlBuilder()
               .UseConnectionString(FreeSql.DataType.MySql, "Data Source=123.207.16.102;Port=23306;User ID=root;Password=qwe369258;Initial Catalog=sysCoreData;Charset=utf8;SslMode=none;Max pool size=5")
               .Build())
                {

                    var tables = fsql.DbFirst.GetTablesByDatabase();
                    var res = Curd.Templates.Select.ToOne();
                    var model = Newtonsoft.Json.JsonConvert.DeserializeObject<object>(strjson);
                    foreach (var table in tables)
                    {

                        var RazorModel = new RazorModel(fsql)
                        {
                            TableName = GetCsEntityName(table.Name)
                        };                      
                        var resHtml = Engine.Razor.RunCompile(res.Code, Guid.NewGuid().ToString("N"), null, new { fsql, table });
                        if (!Directory.Exists(path))
                        {
                            Directory.CreateDirectory(path);
                        }
                        StringBuilder plus = new StringBuilder();
                        plus.AppendLine("//------------------------------------------------------------------------------");
                        plus.AppendLine("// <auto-generated>");
                        plus.AppendLine("//     此代码由工具生成。");
                        plus.AppendLine("//     运行时版本:" + Environment.Version.ToString());
                        plus.AppendLine("//     Website: http://www.freesql.net");
                        plus.AppendLine("//     对此文件的更改可能会导致不正确的行为，并且如果");
                        plus.AppendLine("//     重新生成代码，这些更改将会丢失。");
                        plus.AppendLine("// </auto-generated>");
                        plus.AppendLine("//------------------------------------------------------------------------------");
                        plus.Append(resHtml);
                        plus.AppendLine();
                        File.WriteAllText($"{path}\\{table.Name}.cs", plus.ToString());
                    }
                }
                Process.Start(path);
                return "全部任务生成完成";

            }
            catch (Exception ex)
            {
                return "生成时发生异常,请检查模版代码.";
            }

        }








        protected string UFString(string text)
        {
            text = Regex.Replace(text, @"[^\w]", "_");
            if (text.Length <= 1) return text.ToUpper();
            else return text.Substring(0, 1).ToUpper() + text.Substring(1, text.Length - 1);
        }
        protected string LFString(string text)
        {
            if (text.Length <= 1) return text.ToLower();
            else return text.Substring(0, 1).ToLower() + text.Substring(1, text.Length - 1);
        }
        protected string GetCsEntityName(string dbname)
        {
            var name = Regex.Replace(dbname.TrimStart('@', '.'), @"[^\w]", "_");
            name = char.IsLetter(name, 0) ? name : string.Concat("_", name);
            if (true) name = UFString(name);
            if (true) name = UFString(name.ToLower());
            if (true) name = name.ToLower();
            if (true) name = string.Join("", name.Split('_').Select(a => UFString(a)));
            return name;
        }

    }
}