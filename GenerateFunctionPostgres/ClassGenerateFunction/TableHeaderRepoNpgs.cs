using Dapper;
using GenerateFunctionPostgres.Models;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace GenerateFunctionPostgres.ClassGenerateFunction
{
    public class TableHeaderRepoNpgs
    {
        private string _connectionString;
        public IDbConnection DBconnection;
        public TableHeaderRepoNpgs(string ConnectionString = "")
        {
            _connectionString = ConnectionString;
            DBconnection = new NpgsqlConnection(ConnectionString);
        }

        public List<table_header> GetList()
        {
            List<table_header> tt = new List<table_header>();
            using (IDbConnection conn = new NpgsqlConnection(_connectionString))
            {
                try
                {
                    string Query = @"  SELECT 
                                          header_id,  url,  file_name, title,
                                          line_no,  level,  sp_i,
                                          sp_u,  sp_s,  sp_d,
                                          sp_process,  table_name,  form_type, relation_param,
                                          module_seq,  event_seq,  option_seq,
                                          page_master_seq,  user_input,  user_edit,
                                          time_input,  time_edit
                                      FROM public.ss_table_header 
                                      WHERE form_type is not null
                                      Order by url ASC,line_no ASC,level ASC;";
                    conn.Open();
                    tt = conn.Query<table_header>(Query).ToList();
                }
                catch (Exception ex)
                {
                    throw (ex);
                }
                finally
                {
                    if (conn.State == ConnectionState.Open) conn.Close();
                }
            }
            return tt;
        }

        public List<table_header> GetListBy(string ParamWhere)
        {
            List<table_header> tt = new List<table_header>();
            using (IDbConnection conn = new NpgsqlConnection(_connectionString))
            {
                try
                {
                    string Query = string.Format(@"  SELECT 
                                          header_id,  url,  file_name, title,
                                          line_no,  level,  sp_i,
                                          sp_u,  sp_s,  sp_d,
                                          sp_process,  table_name,  form_type, relation_param,
                                          module_seq,  event_seq,  option_seq,
                                          page_master_seq,  user_input,  user_edit,
                                          time_input,  time_edit,       relation_type
                                      FROM public.ss_table_header 
                                      WHERE form_type is not null  
                                      AND {0}
                                      Order by url ASC,line_no ASC,level ASC;", ParamWhere);
                    conn.Open();
                    tt = conn.Query<table_header>(Query).ToList();
                }
                catch (Exception ex)
                {
                    throw (ex);
                }
                finally
                {
                    if (conn.State == ConnectionState.Open) conn.Close();
                }
            }
            return tt;
        }

        public List<table_detail> GetListDetailById(int header_id)
        {
            List<table_detail> tt = new List<table_detail>();
            using (IDbConnection conn = new NpgsqlConnection(_connectionString))
            {
                try
                {
                    string Query = string.Format(@" SELECT 
                                      detail_id,  header_id,  position,
                                      row_no,  column_name,  column_label,
                                      column_type,  column_value,  corder,
                                      max_length,  lookup_cd,  lookup_table,
                                      lookup_db,  lookup_db_descs,  lookup_db_parameter,
                                      lookup_initial_where,  is_required,  is_visible,
                                      is_key,  is_protected,  user_input,
                                      user_edit,  time_input,  time_edit,  table_name, cmaster_url,
                                      running_no_status, running_cd_column_spec, running_cd_table_name
                                    FROM 
                                      public.ss_table_detail 
                                    WHERE header_id = {0}
                                      order by position ASC,row_no asc;",header_id);
                    conn.Open();
                    //tt = conn.Query<table_detail>(Query, new { header_id = header_id }).ToList();
                    tt = conn.Query<table_detail>(Query).ToList();
                }
                catch (Exception ex)
                {
                    throw (ex);
                }
                finally
                {
                    if (conn.State == ConnectionState.Open) conn.Close();
                }
            }
            return tt;
        }

    }
}
