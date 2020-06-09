/*Insert*/
CREATE OR REPLACE FUNCTION public.fmk_marketing_team_i (
     p_ss_portfolio_id integer,
     p_user_id varchar,
     p_parent_marketing_id varchar,
     p_child_marketing_id varchar,
     p_user_input varchar
)
RETURNS TABLE (
  row_id integer
) AS
$body$
declare v_count integer;
begin

  		INSERT INTO 
  public.mk_marketing_team
    (
          ss_portfolio_id, 
          user_id, 
          parent_marketing_id, 
          child_marketing_id, 
          user_input, 
          user_edit
    )
  VALUES (
          p_ss_portfolio_id,
          p_user_id,
          p_parent_marketing_id,
          p_child_marketing_id,
          p_user_input,
          p_user_input
  );
    
    RETURN QUERY
    SELECT currval(pg_get_serial_sequence('mk_marketing_team', 'mk_marketing_team_id'))::integer as row_id;
end;
$body$
LANGUAGE 'plpgsql'
VOLATILE
CALLED ON NULL INPUT
SECURITY INVOKER
COST 100;


/*Update*/
CREATE OR REPLACE FUNCTION public.fmk_marketing_team_u (
     p_mk_marketing_team_id integer,
     p_ss_portfolio_id integer,
     p_user_id varchar,
     p_parent_marketing_id varchar,
     p_child_marketing_id varchar,
     p_lastupdatestamp integer,
     p_user_edit varchar
)
RETURNS integer AS
$body$
declare v_count integer;
begin	
    
         
        UPDATE  public.mk_marketing_team set
               ss_portfolio_id = p_ss_portfolio_id,
               user_id = p_user_id,
               parent_marketing_id = p_parent_marketing_id,
               child_marketing_id = p_child_marketing_id,
               user_edit = p_user_edit,
               time_edit = now()::timestamp 	 
        WHERE xmin::text::integer = p_lastupdatestamp
        AND mk_marketing_team_id = p_mk_marketing_team_id;
    
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
CREATE OR REPLACE FUNCTION public.fmk_marketing_team_d (
     p_row_id integer,
     p_lastupdatestamp integer
)
RETURNS integer AS
$body$
Declare v_COUNT integer;
begin

	DELETE FROM mk_marketing_team
    WHERE xmin::text::integer = p_lastupdatestamp   
		and mk_marketing_team_id = p_row_id;
                   
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
CREATE OR REPLACE FUNCTION public.fmk_marketing_team_s (
     p_row_id integer,
	 p_lastupdatestamp integer
)
RETURNS TABLE (
     mk_marketing_team_id integer,
     ss_portfolio_id integer,
     user_id varchar,
     parent_marketing_id varchar,
     child_marketing_id varchar,
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
               a.mk_marketing_team_id,
               a.ss_portfolio_id,
               a.user_id,
               a.parent_marketing_id,
               a.child_marketing_id,
               a.user_input,
               a.user_edit,
               a.time_input,
               a.time_edit,
               a.mk_marketing_team_id as row_id,
               a.xmin::text::integer as lastupdatestamp 
          FROM mk_marketing_team a 
          WHERE a.mk_marketing_team_id = p_row_id 
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
create view vmk_marketing_team
AS
          SELECT 
               a.mk_marketing_team_id,
               a.ss_portfolio_id,
               a.user_id,
               a.parent_marketing_id,
               a.child_marketing_id,
               a.user_input,
               a.user_edit,
               a.time_input,
               a.time_edit,
               a.mk_marketing_team_id as row_id,
               a.xmin::text::integer as lastupdatestamp 
          FROM mk_marketing_team a 
          WHERE a.mk_marketing_team_id = p_row_id 
          AND a.xmin::text::integer = p_lastupdatestamp  
;

