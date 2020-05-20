/*Insert*/
CREATE OR REPLACE FUNCTION public.fss_group_i (
     p_ss_portfolio_id integer,
     p_descs varchar,
     p_short_descs varchar,
     p_user_type varchar,
     p_user_input varchar
)
RETURNS TABLE (
  row_id integer
) AS
$body$
declare v_count integer;
begin

  		INSERT INTO 
  public.ss_group
    (
          ss_portfolio_id, 
          descs, 
          short_descs, 
          user_type, 
          user_input, 
          user_edit
    )
  VALUES (
          p_ss_portfolio_id,
          p_descs,
          p_short_descs,
          p_user_type,
          p_user_input,
          p_user_input
  );
    
    RETURN QUERY
    SELECT currval(pg_get_serial_sequence('ss_group', 'ss_group_id'))::integer as row_id;
end;
$body$
LANGUAGE 'plpgsql'
VOLATILE
CALLED ON NULL INPUT
SECURITY INVOKER
COST 100;


/*Update*/
CREATE OR REPLACE FUNCTION public.fss_group_u (
     p_ss_group_id integer,
     p_ss_portfolio_id integer,
     p_descs varchar,
     p_short_descs varchar,
     p_user_type varchar,
     p_lastupdatestamp integer,
     p_user_edit varchar
)
RETURNS integer AS
$body$
declare v_count integer;
begin	
    
         
        UPDATE  public.ss_group set
               ss_portfolio_id = p_ss_portfolio_id,
               descs = p_descs,
               short_descs = p_short_descs,
               user_type = p_user_type,
               user_edit = p_user_edit,
               time_edit = now()::timestamp 	 
        WHERE xmin::text::integer = p_lastupdatestamp
        AND ss_group_id = p_ss_group_id;
    
    GET DIAGNOSTICS v_count = ROW_COUNT;
    return v_count;
end;
$body$
LANGUAGE 'plpgsql'
VOLATILE
CALLED ON NULL INPUT
SECURITY INVOKER
COST 100;


/*Delete*/
CREATE OR REPLACE FUNCTION public.fss_group_d (
     p_row_id integer,
     p_lastupdatestamp integer
)
RETURNS integer AS
$body$
Declare v_COUNT integer;
begin

	DELETE FROM ss_group
    WHERE xmin::text::integer = p_lastupdatestamp   
		and ss_group_id = p_row_id;
                   
    GET DIAGNOSTICS v_COUNT = ROW_COUNT; 
    
  RETURN v_COUNT;
end;
$body$
LANGUAGE 'plpgsql'
VOLATILE
CALLED ON NULL INPUT
SECURITY INVOKER
COST 100;

/*Select*/
CREATE OR REPLACE FUNCTION public.fss_group_s (
     p_row_id integer,
	 p_lastupdatestamp integer
)
RETURNS TABLE (
     ss_group_id integer,
     ss_portfolio_id integer,
     descs varchar,
     short_descs varchar,
     user_type varchar,
     user_input varchar,
     user_edit varchar,
     time_input timestamp,
     time_edit timestamp,
     row_id integer,
     lastupdatestamp integer
) AS
$body$
DECLARE v_id INTEGER; 
BEGIN 	
      RETURN QUERY                
          SELECT 
               a.ss_group_id,
               a.ss_portfolio_id,
               a.descs,
               a.short_descs,
               a.user_type,
               a.user_input,
               a.user_edit,
               a.time_input,
               a.time_edit,
               a.ss_group_id as row_id,
               a.xmin::text::integer as lastupdatestamp 
          FROM ss_group a 
          WHERE a.ss_group_id = p_row_id 
          AND a.xmin::text::integer = p_lastupdatestamp  
;
	 	  
END;
$body$
LANGUAGE 'plpgsql'
VOLATILE
CALLED ON NULL INPUT
SECURITY INVOKER
COST 100 ROWS 1000;

/*View*/
create view vss_group
AS
          SELECT 
               a.ss_group_id,
               a.ss_portfolio_id,
               a.descs,
               a.short_descs,
               a.user_type,
               a.user_input,
               a.user_edit,
               a.time_input,
               a.time_edit,
               a.ss_group_id as row_id,
               a.xmin::text::integer as lastupdatestamp 
          FROM ss_group a 
          WHERE a.ss_group_id = p_row_id 
          AND a.xmin::text::integer = p_lastupdatestamp  
;

