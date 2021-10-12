/*Insert*/
CREATE OR REPLACE FUNCTION public.ffm_fleet_mstr_i (
     p_ss_portfolio_id integer,
     p_bpkb_no varchar,
     p_license_plate_no varchar,
     p_license_plate_expiry_date timestamp,
     p_fm_fleet_brand_id integer,
     p_fm_fleet_type_id integer,
     p_fm_fleet_carosery_id integer,
     p_stnk_no varchar,
     p_stnk_expiry_date timestamp,
     p_kir varchar,
     p_kir_expiry_date timestamp,
     p_fm_driver_id integer,
     p_fm_driver_id2 integer,
     p_capacity_kgs numeric,
     p_capacity_cbm numeric,
     p_millage numeric,
     p_remarks varchar,
     p_user_input varchar,
     p_op_vendor_gps_id integer,
     p_fleet_gps_id varchar,
     p_fleet_status varchar,
     p_last_cm_zone_id integer,
     p_ownership varchar,
     p_reason varchar,
     p_fleet_status_new varchar,
     p_available_date timestamp,
     p_production_year integer,
     p_length integer,
     p_width integer,
     p_height integer,
     p_fm_fleet_model_type_id integer,
     p_chassis_no varchar,
     p_depreciation_amt numeric,
     p_expected_income_month_amt numeric,
     p_average_maintenance_month_amt numeric,
     p_expected_mileage_month_amt numeric
)
RETURNS TABLE (
  row_id integer
) AS
$body$
declare v_count integer;
begin

  		INSERT INTO 
  public.fm_fleet_mstr
    (
          ss_portfolio_id, 
          bpkb_no, 
          license_plate_no, 
          license_plate_expiry_date, 
          fm_fleet_brand_id, 
          fm_fleet_type_id, 
          fm_fleet_carosery_id, 
          stnk_no, 
          stnk_expiry_date, 
          kir, 
          kir_expiry_date, 
          fm_driver_id, 
          fm_driver_id2, 
          capacity_kgs, 
          capacity_cbm, 
          millage, 
          remarks, 
          user_input, 
          user_edit, 
          op_vendor_gps_id, 
          fleet_gps_id, 
          fleet_status, 
          last_cm_zone_id, 
          ownership, 
          reason, 
          fleet_status_new, 
          available_date, 
          production_year, 
          length, 
          width, 
          height, 
          fm_fleet_model_type_id, 
          chassis_no, 
          depreciation_amt, 
          expected_income_month_amt, 
          average_maintenance_month_amt, 
          expected_mileage_month_amt
    )
  VALUES (
          p_ss_portfolio_id,
          p_bpkb_no,
          p_license_plate_no,
          p_license_plate_expiry_date,
          p_fm_fleet_brand_id,
          p_fm_fleet_type_id,
          p_fm_fleet_carosery_id,
          p_stnk_no,
          p_stnk_expiry_date,
          p_kir,
          p_kir_expiry_date,
          p_fm_driver_id,
          p_fm_driver_id2,
          p_capacity_kgs,
          p_capacity_cbm,
          p_millage,
          p_remarks,
          p_user_input,
          p_user_input,
          p_op_vendor_gps_id,
          p_fleet_gps_id,
          p_fleet_status,
          p_last_cm_zone_id,
          p_ownership,
          p_reason,
          p_fleet_status_new,
          p_available_date,
          p_production_year,
          p_length,
          p_width,
          p_height,
          p_fm_fleet_model_type_id,
          p_chassis_no,
          p_depreciation_amt,
          p_expected_income_month_amt,
          p_average_maintenance_month_amt,
          p_expected_mileage_month_amt
  );
    
    RETURN QUERY
    SELECT currval(pg_get_serial_sequence('fm_fleet_mstr', 'fm_fleet_mstr_id'))::integer as row_id;
end;
$body$
LANGUAGE 'plpgsql'
VOLATILE
CALLED ON NULL INPUT
SECURITY INVOKER
COST 100;


/*Update*/
CREATE OR REPLACE FUNCTION public.ffm_fleet_mstr_u (
     p_fm_fleet_mstr_id integer,
     p_ss_portfolio_id integer,
     p_bpkb_no varchar,
     p_license_plate_no varchar,
     p_license_plate_expiry_date timestamp,
     p_fm_fleet_brand_id integer,
     p_fm_fleet_type_id integer,
     p_fm_fleet_carosery_id integer,
     p_stnk_no varchar,
     p_stnk_expiry_date timestamp,
     p_kir varchar,
     p_kir_expiry_date timestamp,
     p_fm_driver_id integer,
     p_fm_driver_id2 integer,
     p_capacity_kgs numeric,
     p_capacity_cbm numeric,
     p_millage numeric,
     p_remarks varchar,
     p_lastupdatestamp integer,
     p_user_edit varchar,
     p_op_vendor_gps_id integer,
     p_fleet_gps_id varchar,
     p_fleet_status varchar,
     p_last_cm_zone_id integer,
     p_ownership varchar,
     p_reason varchar,
     p_fleet_status_new varchar,
     p_available_date timestamp,
     p_production_year integer,
     p_length integer,
     p_width integer,
     p_height integer,
     p_fm_fleet_model_type_id integer,
     p_chassis_no varchar,
     p_depreciation_amt numeric,
     p_expected_income_month_amt numeric,
     p_average_maintenance_month_amt numeric,
     p_expected_mileage_month_amt numeric
)
RETURNS integer AS
$body$
declare v_count integer;
begin	
    
         
        UPDATE  public.fm_fleet_mstr set
               ss_portfolio_id = p_ss_portfolio_id,
               bpkb_no = p_bpkb_no,
               license_plate_no = p_license_plate_no,
               license_plate_expiry_date = p_license_plate_expiry_date,
               fm_fleet_brand_id = p_fm_fleet_brand_id,
               fm_fleet_type_id = p_fm_fleet_type_id,
               fm_fleet_carosery_id = p_fm_fleet_carosery_id,
               stnk_no = p_stnk_no,
               stnk_expiry_date = p_stnk_expiry_date,
               kir = p_kir,
               kir_expiry_date = p_kir_expiry_date,
               fm_driver_id = p_fm_driver_id,
               fm_driver_id2 = p_fm_driver_id2,
               capacity_kgs = p_capacity_kgs,
               capacity_cbm = p_capacity_cbm,
               millage = p_millage,
               remarks = p_remarks,
               user_edit = p_user_edit,
               time_edit = now()::timestamp ,
               op_vendor_gps_id = p_op_vendor_gps_id,
               fleet_gps_id = p_fleet_gps_id,
               fleet_status = p_fleet_status,
               last_cm_zone_id = p_last_cm_zone_id,
               ownership = p_ownership,
               reason = p_reason,
               fleet_status_new = p_fleet_status_new,
               available_date = p_available_date,
               production_year = p_production_year,
               length = p_length,
               width = p_width,
               height = p_height,
               fm_fleet_model_type_id = p_fm_fleet_model_type_id,
               chassis_no = p_chassis_no,
               depreciation_amt = p_depreciation_amt,
               expected_income_month_amt = p_expected_income_month_amt,
               average_maintenance_month_amt = p_average_maintenance_month_amt,
               expected_mileage_month_amt = p_expected_mileage_month_amt	 
        WHERE xmin::text::integer = p_lastupdatestamp
        AND fm_fleet_mstr_id = p_fm_fleet_mstr_id;
    
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
CREATE OR REPLACE FUNCTION public.ffm_fleet_mstr_d (
     p_row_id integer,
     p_lastupdatestamp integer
)
RETURNS integer AS
$body$
Declare v_COUNT integer;
begin

	DELETE FROM fm_fleet_mstr
    WHERE xmin::text::integer = p_lastupdatestamp   
		and fm_fleet_mstr_id = p_row_id;
                   
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
CREATE OR REPLACE FUNCTION public.ffm_fleet_mstr_s (
     p_row_id integer,
	 p_lastupdatestamp integer
)
RETURNS TABLE (
     fm_fleet_mstr_id integer,
     ss_portfolio_id integer,
     bpkb_no varchar,
     license_plate_no varchar,
     license_plate_expiry_date timestamp,
     fm_fleet_brand_id integer,
     fm_fleet_type_id integer,
     fm_fleet_carosery_id integer,
     stnk_no varchar,
     stnk_expiry_date timestamp,
     kir varchar,
     kir_expiry_date timestamp,
     fm_driver_id integer,
     fm_driver_id2 integer,
     capacity_kgs numeric,
     capacity_cbm numeric,
     millage numeric,
     remarks varchar,
     user_input varchar,
     user_edit varchar,
     time_input timestamp,
     time_edit timestamp,
     op_vendor_gps_id integer,
     fleet_gps_id varchar,
     fleet_status varchar,
     last_cm_zone_id integer,
     ownership varchar,
     reason varchar,
     fleet_status_new varchar,
     available_date timestamp,
     production_year integer,
     length integer,
     width integer,
     height integer,
     fm_fleet_model_type_id integer,
     chassis_no varchar,
     depreciation_amt numeric,
     expected_income_month_amt numeric,
     average_maintenance_month_amt numeric,
     expected_mileage_month_amt numeric,
     row_id integer,
     lastupdatestamp integer
) AS
$body$
DECLARE v_id INTEGER; 
BEGIN 	
      RETURN QUERY                
          SELECT 
               a.fm_fleet_mstr_id,
               a.ss_portfolio_id,
               a.bpkb_no,
               a.license_plate_no,
               a.license_plate_expiry_date,
               a.fm_fleet_brand_id,
               a.fm_fleet_type_id,
               a.fm_fleet_carosery_id,
               a.stnk_no,
               a.stnk_expiry_date,
               a.kir,
               a.kir_expiry_date,
               a.fm_driver_id,
               a.fm_driver_id2,
               a.capacity_kgs,
               a.capacity_cbm,
               a.millage,
               a.remarks,
               a.user_input,
               a.user_edit,
               a.time_input,
               a.time_edit,
               a.op_vendor_gps_id,
               a.fleet_gps_id,
               a.fleet_status,
               a.last_cm_zone_id,
               a.ownership,
               a.reason,
               a.fleet_status_new,
               a.available_date,
               a.production_year,
               a.length,
               a.width,
               a.height,
               a.fm_fleet_model_type_id,
               a.chassis_no,
               a.depreciation_amt,
               a.expected_income_month_amt,
               a.average_maintenance_month_amt,
               a.expected_mileage_month_amt,
               a.fm_fleet_mstr_id as row_id,
               a.xmin::text::integer as lastupdatestamp 
          FROM fm_fleet_mstr a 
          WHERE a.fm_fleet_mstr_id = p_row_id 
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
create view vfm_fleet_mstr
AS
          SELECT 
               a.fm_fleet_mstr_id,
               a.ss_portfolio_id,
               a.bpkb_no,
               a.license_plate_no,
               a.license_plate_expiry_date,
               a.fm_fleet_brand_id,
               a.fm_fleet_type_id,
               a.fm_fleet_carosery_id,
               a.stnk_no,
               a.stnk_expiry_date,
               a.kir,
               a.kir_expiry_date,
               a.fm_driver_id,
               a.fm_driver_id2,
               a.capacity_kgs,
               a.capacity_cbm,
               a.millage,
               a.remarks,
               a.user_input,
               a.user_edit,
               a.time_input,
               a.time_edit,
               a.op_vendor_gps_id,
               a.fleet_gps_id,
               a.fleet_status,
               a.last_cm_zone_id,
               a.ownership,
               a.reason,
               a.fleet_status_new,
               a.available_date,
               a.production_year,
               a.length,
               a.width,
               a.height,
               a.fm_fleet_model_type_id,
               a.chassis_no,
               a.depreciation_amt,
               a.expected_income_month_amt,
               a.average_maintenance_month_amt,
               a.expected_mileage_month_amt,
               a.fm_fleet_mstr_id as row_id,
               a.xmin::text::integer as lastupdatestamp 
          FROM fm_fleet_mstr a 
          WHERE a.fm_fleet_mstr_id = p_row_id 
          AND a.xmin::text::integer = p_lastupdatestamp  
;

