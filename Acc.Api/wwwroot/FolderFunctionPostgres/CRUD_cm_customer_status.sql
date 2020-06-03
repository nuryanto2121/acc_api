/*Insert*/
CREATE OR REPLACE FUNCTION public.fcm_customer_status_i (
     p_ss_portfolio_id integer,
     p_descs varchar,
     p_user_input varchar
)
RETURNS TABLE (
  row_id integer
) AS
$body$
declare v_count integer;
begin

  		INSERT INTO 
  public.cm_customer_status
    (
          ss_portfolio_id, 
          descs, 
          user_input, 
          user_edit
    )
  VALUES (
          p_ss_portfolio_id,
          p_descs,
          p_user_input,
          p_user_input
  );
    
    RETURN QUERY
    SELECT currval(pg_get_serial_sequence('cm_customer_status', 'cm_customer_status_id'))::integer as row_id;
end;
$body$
LANGUAGE 'plpgsql'
VOLATILE
CALLED ON NULL INPUT
SECURITY INVOKER
COST 100;


/*Update*/
CREATE OR REPLACE FUNCTION public.fcm_customer_status_u (
     p_cm_customer_status_id integer,
     p_ss_portfolio_id integer,
     p_descs varchar,
     p_lastupdatestamp integer,
     p_user_edit varchar
)
RETURNS integer AS
$body$
declare v_count integer;
begin	
    
         
        UPDATE  public.cm_customer_status set
               ss_portfolio_id = p_ss_portfolio_id,
               descs = p_descs,
               user_edit = p_user_edit,
               time_edit = now()::timestamp 	 
        WHERE xmin::text::integer = p_lastupdatestamp
        AND cm_customer_status_id = p_cm_customer_status_id;
    
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
CREATE OR REPLACE FUNCTION public.fcm_customer_status_d (
     p_row_id integer,
     p_lastupdatestamp integer
)
RETURNS integer AS
$body$
Declare v_COUNT integer;
begin

	DELETE FROM cm_customer_status
    WHERE xmin::text::integer = p_lastupdatestamp   
		and cm_customer_status_id = p_row_id;
                   
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
CREATE OR REPLACE FUNCTION public.fcm_customer_status_s (
     p_row_id integer,
	 p_lastupdatestamp integer
)
RETURNS TABLE (
     cm_customer_status_id integer,
     ss_portfolio_id integer,
     descs varchar,
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
               a.cm_customer_status_id,
               a.ss_portfolio_id,
               a.descs,
               a.user_input,
               a.user_edit,
               a.time_input,
               a.time_edit,
               a.cm_customer_status_id as row_id,
               a.xmin::text::integer as lastupdatestamp 
          FROM cm_customer_status a 
          WHERE a.cm_customer_status_id = p_row_id 
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
create view vcm_customer_status
AS
          SELECT 
               a.cm_customer_status_id,
               a.ss_portfolio_id,
               a.descs,
               a.user_input,
               a.user_edit,
               a.time_input,
               a.time_edit,
               a.cm_customer_status_id as row_id,
               a.xmin::text::integer as lastupdatestamp 
          FROM cm_customer_status a 
          WHERE a.cm_customer_status_id = p_row_id 
          AND a.xmin::text::integer = p_lastupdatestamp  
;

