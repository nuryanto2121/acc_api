/*Insert*/
CREATE OR REPLACE FUNCTION public.ffm_fleet_mstr_status_his_i (
     p_fleet_id integer,
     p_status varchar,
     p_status_date timestamp,
     p_reasong varchar,
     p_user_input varchar
)
RETURNS TABLE (
  row_id integer
) AS
$body$
declare v_count integer;
begin

  		INSERT INTO 
  public.fm_fleet_mstr_status_his
    (
          fleet_id, 
          status, 
          status_date, 
          reasong, 
          user_input, 
          user_edit
    )
  VALUES (
          p_fleet_id,
          p_status,
          p_status_date,
          p_reasong,
          p_user_input,
          p_user_input
  );
    
    RETURN QUERY
    SELECT currval(pg_get_serial_sequence('fm_fleet_mstr_status_his', 'fm_fleet_mstr_status_his_id'))::integer as row_id;
end;
$body$
LANGUAGE 'plpgsql'
VOLATILE
CALLED ON NULL INPUT
SECURITY INVOKER
COST 100;


/*Update*/
CREATE OR REPLACE FUNCTION public.ffm_fleet_mstr_status_his_u (
     p_fm_fleet_mstr_status_his_id integer,
     p_fleet_id integer,
     p_status varchar,
     p_status_date timestamp,
     p_reasong varchar,
     p_lastupdatestamp integer,
     p_user_edit varchar
)
RETURNS integer AS
$body$
declare v_count integer;
begin	
    
         
        UPDATE  public.fm_fleet_mstr_status_his set
               fleet_id = p_fleet_id,
               status = p_status,
               status_date = p_status_date,
               reasong = p_reasong,
               user_edit = p_user_edit,
               time_edit = now()::timestamp 	 
        WHERE xmin::text::integer = p_lastupdatestamp
        AND fm_fleet_mstr_status_his_id = p_fm_fleet_mstr_status_his_id;
    
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
CREATE OR REPLACE FUNCTION public.ffm_fleet_mstr_status_his_d (
     p_row_id integer,
     p_lastupdatestamp integer
)
RETURNS integer AS
$body$
Declare v_COUNT integer;
begin

	DELETE FROM fm_fleet_mstr_status_his
    WHERE xmin::text::integer = p_lastupdatestamp   
		and fm_fleet_mstr_status_his_id = p_row_id;
                   
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
CREATE OR REPLACE FUNCTION public.ffm_fleet_mstr_status_his_s (
     p_row_id integer,
	 p_lastupdatestamp integer
)
RETURNS TABLE (
     fm_fleet_mstr_status_his_id integer,
     fleet_id integer,
     status varchar,
     status_date timestamp,
     reasong varchar,
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
               a.fm_fleet_mstr_status_his_id,
               a.fleet_id,
               a.status,
               a.status_date,
               a.reasong,
               a.user_input,
               a.user_edit,
               a.time_input,
               a.time_edit,
               a.fm_fleet_mstr_status_his_id as row_id,
               a.xmin::text::integer as lastupdatestamp 
          FROM fm_fleet_mstr_status_his a 
          WHERE a.fm_fleet_mstr_status_his_id = p_row_id 
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
create view vfm_fleet_mstr_status_his
AS
          SELECT 
               a.fm_fleet_mstr_status_his_id,
               a.fleet_id,
               a.status,
               a.status_date,
               a.reasong,
               a.user_input,
               a.user_edit,
               a.time_input,
               a.time_edit,
               a.fm_fleet_mstr_status_his_id as row_id,
               a.xmin::text::integer as lastupdatestamp 
          FROM fm_fleet_mstr_status_his a 
          WHERE a.fm_fleet_mstr_status_his_id = p_row_id 
          AND a.xmin::text::integer = p_lastupdatestamp  
;

