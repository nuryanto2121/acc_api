/*Insert*/
CREATE OR REPLACE FUNCTION public.fss_menu_button_i (
     p_menu_url varchar,
     p_button_id varchar,
     p_user_input varchar,
     p_option_url varchar,
     p_display_form varchar,
     p_display_button varchar,
     p_sequence_form integer,
     p_ss_menu_dashboard_id integer
)
RETURNS TABLE (
  row_id integer
) AS
$body$
declare v_count integer;
begin

  		INSERT INTO 
  public.ss_menu_button
    (
          menu_url, 
          button_id, 
          user_input, 
          user_edit, 
          option_url, 
          display_form, 
          display_button, 
          sequence_form, 
          ss_menu_dashboard_id
    )
  VALUES (
          p_menu_url,
          p_button_id,
          p_user_input,
          p_user_input,
          p_option_url,
          p_display_form,
          p_display_button,
          p_sequence_form,
          p_ss_menu_dashboard_id
  );
    
    RETURN QUERY
    SELECT currval(pg_get_serial_sequence('ss_menu_button', 'ss_menu_button_id'))::integer as row_id;
end;
$body$
LANGUAGE 'plpgsql'
VOLATILE
CALLED ON NULL INPUT
SECURITY INVOKER
COST 100;


/*Update*/
CREATE OR REPLACE FUNCTION public.fss_menu_button_u (
     p_ss_menu_button_id integer,
     p_menu_url varchar,
     p_button_id varchar,
     p_lastupdatestamp integer,
     p_user_edit varchar,
     p_option_url varchar,
     p_display_form varchar,
     p_display_button varchar,
     p_sequence_form integer,
     p_ss_menu_dashboard_id integer
)
RETURNS integer AS
$body$
declare v_count integer;
begin	
    
         
        UPDATE  public.ss_menu_button set
               menu_url = p_menu_url,
               button_id = p_button_id,
               user_edit = p_user_edit,
               time_edit = now()::timestamp ,
               option_url = p_option_url,
               display_form = p_display_form,
               display_button = p_display_button,
               sequence_form = p_sequence_form,
               ss_menu_dashboard_id = p_ss_menu_dashboard_id	 
        WHERE xmin::text::integer = p_lastupdatestamp
        AND ss_menu_button_id = p_ss_menu_button_id;
    
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
CREATE OR REPLACE FUNCTION public.fss_menu_button_d (
     p_row_id integer,
     p_lastupdatestamp integer
)
RETURNS integer AS
$body$
Declare v_COUNT integer;
begin

	DELETE FROM ss_menu_button
    WHERE xmin::text::integer = p_lastupdatestamp   
		and ss_menu_button_id = p_row_id;
                   
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
CREATE OR REPLACE FUNCTION public.fss_menu_button_s (
     p_row_id integer,
	 p_lastupdatestamp integer
)
RETURNS TABLE (
     ss_menu_button_id integer,
     menu_url varchar,
     button_id varchar,
     user_input varchar,
     user_edit varchar,
     time_input timestamp,
     time_edit timestamp,
     option_url varchar,
     display_form varchar,
     display_button varchar,
     sequence_form integer,
     ss_menu_dashboard_id integer,
     row_id integer,
     lastupdatestamp integer
) AS
$body$
DECLARE v_id INTEGER; 
BEGIN 	
      RETURN QUERY                
          SELECT 
               a.ss_menu_button_id,
               a.menu_url,
               a.button_id,
               a.user_input,
               a.user_edit,
               a.time_input,
               a.time_edit,
               a.option_url,
               a.display_form,
               a.display_button,
               a.sequence_form,
               a.ss_menu_dashboard_id,
               a.ss_menu_button_id as row_id,
               a.xmin::text::integer as lastupdatestamp 
          FROM ss_menu_button a 
          WHERE a.ss_menu_button_id = p_row_id 
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
create view vss_menu_button
AS
          SELECT 
               a.ss_menu_button_id,
               a.menu_url,
               a.button_id,
               a.user_input,
               a.user_edit,
               a.time_input,
               a.time_edit,
               a.option_url,
               a.display_form,
               a.display_button,
               a.sequence_form,
               a.ss_menu_dashboard_id,
               a.ss_menu_button_id as row_id,
               a.xmin::text::integer as lastupdatestamp 
          FROM ss_menu_button a 
          WHERE a.ss_menu_button_id = p_row_id 
          AND a.xmin::text::integer = p_lastupdatestamp  
;

