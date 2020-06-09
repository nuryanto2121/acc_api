/*Insert*/
CREATE OR REPLACE FUNCTION public.fmk_marketing_target_i (
     p_years integer,
     p_month integer,
     p_ftl_amt numeric,
     p_ltl_amt numeric,
     p_project_amt numeric,
     p_rental_amt numeric
)
RETURNS TABLE (
  row_id integer
) AS
$body$
declare v_count integer;
begin

  		INSERT INTO 
  public.mk_marketing_target
    (
          years, 
          month, 
          ftl_amt, 
          ltl_amt, 
          project_amt, 
          rental_amt, 
          user_edit
    )
  VALUES (
          p_years,
          p_month,
          p_ftl_amt,
          p_ltl_amt,
          p_project_amt,
          p_rental_amt,
          p_user_input
  );
    
    RETURN QUERY
    SELECT currval(pg_get_serial_sequence('mk_marketing_target', 'mk_marketing_target_id'))::integer as row_id;
end;
$body$
LANGUAGE 'plpgsql'
VOLATILE
CALLED ON NULL INPUT
SECURITY INVOKER
COST 100;


/*Update*/
CREATE OR REPLACE FUNCTION public.fmk_marketing_target_u (
     p_mk_marketing_team_id integer,
     p_years integer,
     p_month integer,
     p_ftl_amt numeric,
     p_ltl_amt numeric,
     p_project_amt numeric,
     p_rental_amt numeric,
     p_user_edit varchar
)
RETURNS integer AS
$body$
declare v_count integer;
begin	
    
         
        UPDATE  public.mk_marketing_target set
               years = p_years,
               month = p_month,
               ftl_amt = p_ftl_amt,
               ltl_amt = p_ltl_amt,
               project_amt = p_project_amt,
               rental_amt = p_rental_amt,
               user_edit = p_user_edit,
               time_edit = now()::timestamp 	 
        WHERE xmin::text::integer = p_lastupdatestamp
        AND mk_marketing_target_id = p_mk_marketing_target_id;
    
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
CREATE OR REPLACE FUNCTION public.fmk_marketing_target_d (
     p_row_id integer,
     p_lastupdatestamp integer
)
RETURNS integer AS
$body$
Declare v_COUNT integer;
begin

	DELETE FROM mk_marketing_target
    WHERE xmin::text::integer = p_lastupdatestamp   
		and mk_marketing_target_id = p_row_id;
                   
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
CREATE OR REPLACE FUNCTION public.fmk_marketing_target_s (
     p_row_id integer,
	 p_lastupdatestamp integer
)
RETURNS TABLE (
     mk_marketing_team_id integer,
     years integer,
     month integer,
     ftl_amt numeric,
     ltl_amt numeric,
     project_amt numeric,
     rental_amt numeric,
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
               a.years,
               a.month,
               a.ftl_amt,
               a.ltl_amt,
               a.project_amt,
               a.rental_amt,
               a.user_edit,
               a.time_input,
               a.time_edit,
               a.mk_marketing_team_id as row_id,
               a.xmin::text::integer as lastupdatestamp 
          FROM mk_marketing_target a 
          WHERE a.mk_marketing_target_id = p_row_id 
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
create view vmk_marketing_target
AS
          SELECT 
               a.mk_marketing_team_id,
               a.years,
               a.month,
               a.ftl_amt,
               a.ltl_amt,
               a.project_amt,
               a.rental_amt,
               a.user_edit,
               a.time_input,
               a.time_edit,
               a.mk_marketing_team_id as row_id,
               a.xmin::text::integer as lastupdatestamp 
          FROM mk_marketing_target a 
          WHERE a.mk_marketing_target_id = p_row_id 
          AND a.xmin::text::integer = p_lastupdatestamp  
;

