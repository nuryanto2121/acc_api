/*Insert*/
CREATE OR REPLACE FUNCTION public.fss_user_favorite_i (
     p_ss_portfolio_id integer,
     p_user_id varchar,
     p_ss_menu_id integer,
     p_user_input varchar,
     p_row_no integer,
     p_ss_module_id integer
)
RETURNS TABLE (
  row_id integer
) AS
$body$
declare v_count integer;
begin

  		INSERT INTO 
  public.ss_user_favorite
    (
          ss_portfolio_id, 
          user_id, 
          ss_menu_id, 
          user_input, 
          user_edit, 
          row_no, 
          ss_module_id
    )
  VALUES (
          p_ss_portfolio_id,
          p_user_id,
          p_ss_menu_id,
          p_user_input,
          p_user_input,
          p_row_no,
          p_ss_module_id
  );
    
    RETURN QUERY
    SELECT currval(pg_get_serial_sequence('ss_user_favorite', 'ss_user_favorite_id'))::integer as row_id;
end;
$body$
LANGUAGE 'plpgsql'
VOLATILE
CALLED ON NULL INPUT
SECURITY INVOKER
COST 100;


/*Update*/
CREATE OR REPLACE FUNCTION public.fss_user_favorite_u (
     p_ss_user_favorite_id integer,
     p_ss_portfolio_id integer,
     p_user_id varchar,
     p_ss_menu_id integer,
     p_lastupdatestamp integer,
     p_user_edit varchar,
     p_row_no integer,
     p_ss_module_id integer
)
RETURNS integer AS
$body$
declare v_count integer;
begin	
    
         
        UPDATE  public.ss_user_favorite set
               ss_portfolio_id = p_ss_portfolio_id,
               user_id = p_user_id,
               ss_menu_id = p_ss_menu_id,
               user_edit = p_user_edit,
               time_edit = now()::timestamp ,
               row_no = p_row_no,
               ss_module_id = p_ss_module_id	 
        WHERE xmin::text::integer = p_lastupdatestamp
        AND ss_user_favorite_id = p_ss_user_favorite_id;
    
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
CREATE OR REPLACE FUNCTION public.fss_user_favorite_d (
     p_row_id integer,
     p_lastupdatestamp integer
)
RETURNS integer AS
$body$
Declare v_COUNT integer;
begin

	DELETE FROM ss_user_favorite
    WHERE xmin::text::integer = p_lastupdatestamp   
		and ss_user_favorite_id = p_row_id;
                   
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
CREATE OR REPLACE FUNCTION public.fss_user_favorite_s (
     p_row_id integer,
	 p_lastupdatestamp integer
)
RETURNS TABLE (
     ss_user_favorite_id integer,
     ss_portfolio_id integer,
     user_id varchar,
     ss_menu_id integer,
     user_input varchar,
     user_edit varchar,
     time_input timestamp,
     time_edit timestamp,
     row_no integer,
     ss_module_id integer,
     row_id integer,
     lastupdatestamp integer
) AS
$body$
DECLARE v_id INTEGER; 
BEGIN 	
      RETURN QUERY                
          SELECT 
               a.ss_user_favorite_id,
               a.ss_portfolio_id,
               a.user_id,
               a.ss_menu_id,
               a.user_input,
               a.user_edit,
               a.time_input,
               a.time_edit,
               a.row_no,
               a.ss_module_id,
               a.ss_user_favorite_id as row_id,
               a.xmin::text::integer as lastupdatestamp 
          FROM ss_user_favorite a 
          WHERE a.ss_user_favorite_id = p_row_id 
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
create view vss_user_favorite
AS
          SELECT 
               a.ss_user_favorite_id,
               a.ss_portfolio_id,
               a.user_id,
               a.ss_menu_id,
               a.user_input,
               a.user_edit,
               a.time_input,
               a.time_edit,
               a.row_no,
               a.ss_module_id,
               a.ss_user_favorite_id as row_id,
               a.xmin::text::integer as lastupdatestamp 
          FROM ss_user_favorite a 
          WHERE a.ss_user_favorite_id = p_row_id 
          AND a.xmin::text::integer = p_lastupdatestamp  
;

