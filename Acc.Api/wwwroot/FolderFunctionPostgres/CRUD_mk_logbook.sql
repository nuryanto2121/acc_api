/*Insert*/
CREATE OR REPLACE FUNCTION public.fmk_logbook_i (
     p_ss_portfolio_id integer,
     p_cm_contact_id integer,
     p_cm_contact_person_id integer,
     p_appoinment_date timestamp,
     p_logbook_date timestamp,
     p_action_type character,
     p_meeting_address varchar,
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
  public.mk_logbook
    (
          ss_portfolio_id, 
          cm_contact_id, 
          cm_contact_person_id, 
          appoinment_date, 
          logbook_date, 
          action_type, 
          meeting_address, 
          descs, 
          user_input, 
          user_edit
    )
  VALUES (
          p_ss_portfolio_id,
          p_cm_contact_id,
          p_cm_contact_person_id,
          p_appoinment_date,
          p_logbook_date,
          p_action_type,
          p_meeting_address,
          p_descs,
          p_user_input,
          p_user_input
  );
    
    RETURN QUERY
    SELECT currval(pg_get_serial_sequence('mk_logbook', 'mk_logbook_id'))::integer as row_id;
end;
$body$
LANGUAGE 'plpgsql'
VOLATILE
CALLED ON NULL INPUT
SECURITY INVOKER
COST 100;


/*Update*/
CREATE OR REPLACE FUNCTION public.fmk_logbook_u (
     p_mk_logbook_id integer,
     p_ss_portfolio_id integer,
     p_cm_contact_id integer,
     p_cm_contact_person_id integer,
     p_appoinment_date timestamp,
     p_logbook_date timestamp,
     p_action_type character,
     p_meeting_address varchar,
     p_descs varchar,
     p_lastupdatestamp integer,
     p_user_edit varchar
)
RETURNS integer AS
$body$
declare v_count integer;
begin	
    
         
        UPDATE  public.mk_logbook set
               ss_portfolio_id = p_ss_portfolio_id,
               cm_contact_id = p_cm_contact_id,
               cm_contact_person_id = p_cm_contact_person_id,
               appoinment_date = p_appoinment_date,
               logbook_date = p_logbook_date,
               action_type = p_action_type,
               meeting_address = p_meeting_address,
               descs = p_descs,
               user_edit = p_user_edit,
               time_edit = now()::timestamp 	 
        WHERE xmin::text::integer = p_lastupdatestamp
        AND mk_logbook_id = p_mk_logbook_id;
    
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
CREATE OR REPLACE FUNCTION public.fmk_logbook_d (
     p_row_id integer,
     p_lastupdatestamp integer
)
RETURNS integer AS
$body$
Declare v_COUNT integer;
begin

	DELETE FROM mk_logbook
    WHERE xmin::text::integer = p_lastupdatestamp   
		and mk_logbook_id = p_row_id;
                   
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
CREATE OR REPLACE FUNCTION public.fmk_logbook_s (
     p_row_id integer,
	 p_lastupdatestamp integer
)
RETURNS TABLE (
     mk_logbook_id integer,
     ss_portfolio_id integer,
     cm_contact_id integer,
     cm_contact_person_id integer,
     appoinment_date timestamp,
     logbook_date timestamp,
     action_type character,
     meeting_address varchar,
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
               a.mk_logbook_id,
               a.ss_portfolio_id,
               a.cm_contact_id,
               a.cm_contact_person_id,
               a.appoinment_date,
               a.logbook_date,
               a.action_type,
               a.meeting_address,
               a.descs,
               a.user_input,
               a.user_edit,
               a.time_input,
               a.time_edit,
               a.mk_logbook_id as row_id,
               a.xmin::text::integer as lastupdatestamp 
          FROM mk_logbook a 
          WHERE a.mk_logbook_id = p_row_id 
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
create view vmk_logbook
AS
          SELECT 
               a.mk_logbook_id,
               a.ss_portfolio_id,
               a.cm_contact_id,
               a.cm_contact_person_id,
               a.appoinment_date,
               a.logbook_date,
               a.action_type,
               a.meeting_address,
               a.descs,
               a.user_input,
               a.user_edit,
               a.time_input,
               a.time_edit,
               a.mk_logbook_id as row_id,
               a.xmin::text::integer as lastupdatestamp 
          FROM mk_logbook a 
          WHERE a.mk_logbook_id = p_row_id 
          AND a.xmin::text::integer = p_lastupdatestamp  
;

