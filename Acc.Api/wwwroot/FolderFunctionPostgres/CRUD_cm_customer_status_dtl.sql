/*Insert*/
CREATE OR REPLACE FUNCTION public.fcm_customer_status_dtl_i (
     p_cm_customer_status_id integer,
     p_action varchar,
     p_month integer,
     p_freq integer,
     p_user_input varchar
)
RETURNS TABLE (
  row_id integer
) AS
$body$
declare v_count integer;
begin

  		INSERT INTO 
  public.cm_customer_status_dtl
    (
          cm_customer_status_id, 
          action, 
          month, 
          freq, 
          user_input, 
          user_edit
    )
  VALUES (
          p_cm_customer_status_id,
          p_action,
          p_month,
          p_freq,
          p_user_input,
          p_user_input
  );
    
    RETURN QUERY
    SELECT currval(pg_get_serial_sequence('cm_customer_status_dtl', 'cm_customer_status_dtl_id'))::integer as row_id;
end;
$body$
LANGUAGE 'plpgsql'
VOLATILE
CALLED ON NULL INPUT
SECURITY INVOKER
COST 100;


/*Update*/
CREATE OR REPLACE FUNCTION public.fcm_customer_status_dtl_u (
     p_cm_customer_status_dt_id integer,
     p_cm_customer_status_id integer,
     p_action varchar,
     p_month integer,
     p_freq integer,
     p_lastupdatestamp integer,
     p_user_edit varchar
)
RETURNS integer AS
$body$
declare v_count integer;
begin	
    
         
        UPDATE  public.cm_customer_status_dtl set
               cm_customer_status_id = p_cm_customer_status_id,
               action = p_action,
               month = p_month,
               freq = p_freq,
               user_edit = p_user_edit,
               time_edit = now()::timestamp 	 
        WHERE xmin::text::integer = p_lastupdatestamp
        AND cm_customer_status_dtl_id = p_cm_customer_status_dtl_id;
    
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
CREATE OR REPLACE FUNCTION public.fcm_customer_status_dtl_d (
     p_row_id integer,
     p_lastupdatestamp integer
)
RETURNS integer AS
$body$
Declare v_COUNT integer;
begin

	DELETE FROM cm_customer_status_dtl
    WHERE xmin::text::integer = p_lastupdatestamp   
		and cm_customer_status_dtl_id = p_row_id;
                   
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
CREATE OR REPLACE FUNCTION public.fcm_customer_status_dtl_s (
     p_row_id integer,
	 p_lastupdatestamp integer
)
RETURNS TABLE (
     cm_customer_status_dt_id integer,
     cm_customer_status_id integer,
     action varchar,
     month integer,
     freq integer,
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
               a.cm_customer_status_dt_id,
               a.cm_customer_status_id,
               a.action,
               a.month,
               a.freq,
               a.user_input,
               a.user_edit,
               a.time_input,
               a.time_edit,
               a.cm_customer_status_dt_id as row_id,
               a.xmin::text::integer as lastupdatestamp 
          FROM cm_customer_status_dtl a 
          WHERE a.cm_customer_status_dtl_id = p_row_id 
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
create view vcm_customer_status_dtl
AS
          SELECT 
               a.cm_customer_status_dt_id,
               a.cm_customer_status_id,
               a.action,
               a.month,
               a.freq,
               a.user_input,
               a.user_edit,
               a.time_input,
               a.time_edit,
               a.cm_customer_status_dt_id as row_id,
               a.xmin::text::integer as lastupdatestamp 
          FROM cm_customer_status_dtl a 
          WHERE a.cm_customer_status_dtl_id = p_row_id 
          AND a.xmin::text::integer = p_lastupdatestamp  
;

