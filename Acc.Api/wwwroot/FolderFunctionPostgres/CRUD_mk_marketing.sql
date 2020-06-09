/*Insert*/
CREATE OR REPLACE FUNCTION public.fmk_marketing_i (
     p_ss_portfolio_id integer,
     p_user_id varchar,
     p_marketing_id varchar,
     p_name varchar,
     p_nik_id varchar,
     p_address varchar,
     p_email varchar,
     p_hand_phone varchar,
     p_status_active boolean,
     p_join_date timestamp,
     p_monthly_point integer,
     p_monthly_new_prospect integer,
     p_user_input varchar
)
RETURNS TABLE (
  row_id integer
) AS
$body$
declare v_count integer;
begin

  		INSERT INTO 
  public.mk_marketing
    (
          ss_portfolio_id, 
          user_id, 
          marketing_id, 
          name, 
          nik_id, 
          address, 
          email, 
          hand_phone, 
          status_active, 
          join_date, 
          monthly_point, 
          monthly_new_prospect, 
          user_input, 
          user_edit
    )
  VALUES (
          p_ss_portfolio_id,
          p_user_id,
          p_marketing_id,
          p_name,
          p_nik_id,
          p_address,
          p_email,
          p_hand_phone,
          p_status_active,
          p_join_date,
          p_monthly_point,
          p_monthly_new_prospect,
          p_user_input,
          p_user_input
  );
    
    RETURN QUERY
    SELECT currval(pg_get_serial_sequence('mk_marketing', 'mk_marketing_id'))::integer as row_id;
end;
$body$
LANGUAGE 'plpgsql'
VOLATILE
CALLED ON NULL INPUT
SECURITY INVOKER
COST 100;


/*Update*/
CREATE OR REPLACE FUNCTION public.fmk_marketing_u (
     p_mk_marketing_id integer,
     p_ss_portfolio_id integer,
     p_user_id varchar,
     p_marketing_id varchar,
     p_name varchar,
     p_nik_id varchar,
     p_address varchar,
     p_email varchar,
     p_hand_phone varchar,
     p_status_active boolean,
     p_join_date timestamp,
     p_monthly_point integer,
     p_monthly_new_prospect integer,
     p_lastupdatestamp integer,
     p_user_edit varchar
)
RETURNS integer AS
$body$
declare v_count integer;
begin	
    
         
        UPDATE  public.mk_marketing set
               ss_portfolio_id = p_ss_portfolio_id,
               user_id = p_user_id,
               marketing_id = p_marketing_id,
               name = p_name,
               nik_id = p_nik_id,
               address = p_address,
               email = p_email,
               hand_phone = p_hand_phone,
               status_active = p_status_active,
               join_date = p_join_date,
               monthly_point = p_monthly_point,
               monthly_new_prospect = p_monthly_new_prospect,
               user_edit = p_user_edit,
               time_edit = now()::timestamp 	 
        WHERE xmin::text::integer = p_lastupdatestamp
        AND mk_marketing_id = p_mk_marketing_id;
    
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
CREATE OR REPLACE FUNCTION public.fmk_marketing_d (
     p_row_id integer,
     p_lastupdatestamp integer
)
RETURNS integer AS
$body$
Declare v_COUNT integer;
begin

	DELETE FROM mk_marketing
    WHERE xmin::text::integer = p_lastupdatestamp   
		and mk_marketing_id = p_row_id;
                   
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
CREATE OR REPLACE FUNCTION public.fmk_marketing_s (
     p_row_id integer,
	 p_lastupdatestamp integer
)
RETURNS TABLE (
     mk_marketing_id integer,
     ss_portfolio_id integer,
     user_id varchar,
     marketing_id varchar,
     name varchar,
     nik_id varchar,
     address varchar,
     email varchar,
     hand_phone varchar,
     status_active boolean,
     join_date timestamp,
     monthly_point integer,
     monthly_new_prospect integer,
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
               a.mk_marketing_id,
               a.ss_portfolio_id,
               a.user_id,
               a.marketing_id,
               a.name,
               a.nik_id,
               a.address,
               a.email,
               a.hand_phone,
               a.status_active,
               a.join_date,
               a.monthly_point,
               a.monthly_new_prospect,
               a.user_input,
               a.user_edit,
               a.time_input,
               a.time_edit,
               a.mk_marketing_id as row_id,
               a.xmin::text::integer as lastupdatestamp 
          FROM mk_marketing a 
          WHERE a.mk_marketing_id = p_row_id 
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
create view vmk_marketing
AS
          SELECT 
               a.mk_marketing_id,
               a.ss_portfolio_id,
               a.user_id,
               a.marketing_id,
               a.name,
               a.nik_id,
               a.address,
               a.email,
               a.hand_phone,
               a.status_active,
               a.join_date,
               a.monthly_point,
               a.monthly_new_prospect,
               a.user_input,
               a.user_edit,
               a.time_input,
               a.time_edit,
               a.mk_marketing_id as row_id,
               a.xmin::text::integer as lastupdatestamp 
          FROM mk_marketing a 
          WHERE a.mk_marketing_id = p_row_id 
          AND a.xmin::text::integer = p_lastupdatestamp  
;

