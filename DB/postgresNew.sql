PGDMP     8                    x            acc_db    11.4    11.4 }    �           0    0    ENCODING    ENCODING        SET client_encoding = 'UTF8';
                       false            �           0    0 
   STDSTRINGS 
   STDSTRINGS     (   SET standard_conforming_strings = 'on';
                       false            �           0    0 
   SEARCHPATH 
   SEARCHPATH     8   SELECT pg_catalog.set_config('search_path', '', false);
                       false            �           1262    64903    acc_db    DATABASE     �   CREATE DATABASE acc_db WITH TEMPLATE = template0 ENCODING = 'UTF8' LC_COLLATE = 'English_United States.1252' LC_CTYPE = 'English_United States.1252';
    DROP DATABASE acc_db;
             postgres    false            �            1255    64904 G   _getlogin_auth(character varying, character varying, character varying)    FUNCTION     �  CREATE FUNCTION public._getlogin_auth(p_user_id character varying, p_password character varying, p_subportfolio_short_name character varying DEFAULT ''::character varying) RETURNS TABLE(user_id character varying, is_inactive character varying, user_name character varying, password character varying, user_group_short_descs character varying, user_group_descs character varying, subportfolio_short_name character varying, subportfolio_name character varying, portfolio_short_name character varying, portfolio_name character varying, default_language character varying, ss_group_id integer, portfolio_id integer, subportfolio_id integer)
    LANGUAGE plpgsql
    AS $$
DECLARE v_id INTEGER; 
BEGIN 
	
   /* IF p_subportfolio_cd = '' THEN
    	SELECT x.subportfolio_cd into p_subportfolio_cd
        FROM ss_user_subportfolio x
        WHERE x.default_login_status ='Y'
        AND x.user_id iLIKE p_user_id;
    END IF;*/

      RETURN QUERY                
      SELECT    
       a.user_id    
         ,a.is_inactive    
         ,a.user_name    
         ,a.password
         ,sg.short_descs as user_group_short_descs    
         ,sg.descs AS user_group_descs    
         ,c.short_name as  subportfolio_short_name
         ,c.name AS subportfolio_name    
         ,f.short_name as portfolio_short_name    
         ,f.name AS portfolio_name      
         ,COALESCE (a.default_language,'en')  as default_language
         ,a.ss_group_id
         ,a.portfolio_id
         ,a.subportfolio_id
      FROM SS_User a     
      INNER JOIN ss_subportfolio c
       ON a.subportfolio_id = c.ss_subportfolio_id     
      INNER JOIN ss_portfolio f 
       ON a.portfolio_id = f.ss_portfolio_id
      INNER JOIN ss_group sg 
       ON a.ss_group_id = sg.ss_group_id    
      WHERE a.user_id iLIKE p_user_id
      AND a.password = p_password    
      AND a.is_inactive = 'N';    
/*      AND b.subportfolio_cd iLIKE p_subportfolio_cd    */
	 
	
    

END;
$$;
 �   DROP FUNCTION public._getlogin_auth(p_user_id character varying, p_password character varying, p_subportfolio_short_name character varying);
       public       postgres    false            �            1255    64905 !   fss_menu_list_s(integer, integer)    FUNCTION     �  CREATE FUNCTION public.fss_menu_list_s(p_portfolio_id integer, p_group_id integer) RETURNS TABLE(ss_menu_id integer, title character varying, menu_url character varying, menu_type character varying, parent_menu_id integer, icon_class character varying, order_seq integer, level integer, ipath character varying, add_status boolean, edit_status boolean, delete_status boolean)
    LANGUAGE plpgsql
    AS $$
begin
	return query

    WITH RECURSIVE CTE(ss_menu_id,title,menu_url,menu_type,parent_menu_id,icon_class,order_seq,level,ipath) AS (
  			SELECT sm.ss_menu_id,sm.title,sm.menu_url,sm.menu_type,sm.parent_menu_id,sm.icon_class,sm.order_seq,0::integer as level
    		,row_number()OVER(PARTITION BY sm.parent_menu_id order by sm.order_seq)::varchar as ipath
          	FROM ss_menu sm
          	where sm.parent_menu_id = 0
    UNION ALL
    		
            SELECT mn.ss_menu_id,mn.title,mn.menu_url,mn.menu_type,mn.parent_menu_id,mn.icon_class,mn.order_seq,(c.level + 1)::integer as level
	  		,c.ipath||'.'||CAST(row_number()OVER(PARTITION BY mn.parent_menu_id order by mn.order_seq) as varchar(100))::varchar as ipath2
  			FROM ss_menu as mn
        	inner join CTE C
            	on mn.parent_menu_id = c.ss_menu_id
  )
  SELECT ct.*,smg.add_status,smg.edit_status,smg.delete_status
  FROM CTE ct
  inner join ss_menu_group smg on ct.ss_menu_id = smg.ss_menu_id
  where smg.ss_portfolio_id = p_portfolio_id
  and smg.ss_group_id = p_group_id
  order by ct.ipath;
end;
$$;
 R   DROP FUNCTION public.fss_menu_list_s(p_portfolio_id integer, p_group_id integer);
       public       postgres    false            �            1255    64906 =   fss_menu_sort_u(integer, integer, integer, character varying)    FUNCTION     �  CREATE FUNCTION public.fss_menu_sort_u(p_menu_id integer, p_parent_menu_id integer, p_order_seq integer, p_user_edit character varying) RETURNS character varying
    LANGUAGE plpgsql
    AS $$
DECLARE
  v_count integer;
BEGIN
	update public.ss_menu set
    order_seq= p_order_seq,
    user_edit= p_user_edit
    where ss_menu_id = p_menu_id 
    and parent_menu_id = p_parent_menu_id;
    
    GET DIAGNOSTICS v_count = ROW_COUNT;
	RETURN v_count;

END;
$$;
 �   DROP FUNCTION public.fss_menu_sort_u(p_menu_id integer, p_parent_menu_id integer, p_order_seq integer, p_user_edit character varying);
       public       postgres    false            �            1255    64907 w   fss_option_db_i(character varying, character varying, character varying, integer, character varying, character varying)    FUNCTION     �  CREATE FUNCTION public.fss_option_db_i(p_option_url character varying, p_method_api character varying, p_sp character varying, p_line_no integer, p_user_input character varying, p_table_name character varying DEFAULT ''::character varying) RETURNS integer
    LANGUAGE plpgsql
    AS $$
declare v_count integer;
begin

  		INSERT INTO 
  			public.ss_option_db (
              option_url,  	method_api, 		sp,
              line_no,  		table_name,  		user_input,  		
              user_edit
			)
		VALUES (
	  			p_option_url,  	p_method_api, 		p_sp,
              	p_line_no,  		p_table_name,  		p_user_input,  		
              	p_user_input
			);
    
    GET DIAGNOSTICS v_count = ROW_COUNT;
    return v_count;
end;
$$;
 �   DROP FUNCTION public.fss_option_db_i(p_option_url character varying, p_method_api character varying, p_sp character varying, p_line_no integer, p_user_input character varying, p_table_name character varying);
       public       postgres    false            �            1255    64908 �  fss_user_log_i(timestamp without time zone, character varying, character varying, character varying, character varying, character varying, timestamp without time zone, timestamp without time zone, character varying, character varying, character varying, character varying, timestamp without time zone, timestamp without time zone, character varying, timestamp without time zone, character varying)    FUNCTION     �  CREATE FUNCTION public.fss_user_log_i(p_log_status timestamp without time zone, p_user_id character varying, p_user_group character varying, p_user_name character varying, p_email character varying, p_user_level character varying, p_expired_date timestamp without time zone, p_login_date timestamp without time zone, p_status_login character varying, p_is_inactive character varying, p_user_input character varying, p_user_edit character varying, p_time_input timestamp without time zone, p_time_edit timestamp without time zone, p_ip_address character varying, p_logout_date timestamp without time zone, p_token character varying) RETURNS character varying
    LANGUAGE plpgsql
    AS $$
declare
  v_count integer;
  "p_log_sequence_no" integer:=0;
  "p_password" varchar := '';
begin
	select coalesce(max("log_sequence_no"),0) + 1 into "p_log_sequence_no" from public."ss_user_log" ;

    if "p_log_sequence_no" = 0 then
	     "p_log_sequence_no" :=1;
    end if;
    raise notice 'calling cs_create_job(%)', "p_log_sequence_no";

	insert into public."ss_user_log" (
    	"log_status", "log_sequence_no",	"user_id", "user_group", "user_name", "password",	"email", "user_level", "expired_date",
    	"login_date", "status_login", "is_inactive", "user_input", "user_edit", "time_input",
    	"time_edit", "ip_address", "logout_date", "token"
    ) values (
		"p_log_status", "p_log_sequence_no",	"p_user_id", "p_user_group", "p_user_name", "p_password",	"p_email", "p_user_level", "p_expired_date",
    	"p_login_date", "p_status_login", "p_is_inactive", "p_user_input", "p_user_edit", "p_time_input",
    	"p_time_edit", "p_ip_address", "p_logout_date", "p_token"
    );

    get diagnostics v_count = row_count;

    return v_count;
end;
$$;
 w  DROP FUNCTION public.fss_user_log_i(p_log_status timestamp without time zone, p_user_id character varying, p_user_group character varying, p_user_name character varying, p_email character varying, p_user_level character varying, p_expired_date timestamp without time zone, p_login_date timestamp without time zone, p_status_login character varying, p_is_inactive character varying, p_user_input character varying, p_user_edit character varying, p_time_input timestamp without time zone, p_time_edit timestamp without time zone, p_ip_address character varying, p_logout_date timestamp without time zone, p_token character varying);
       public       postgres    false            �            1255    64909 %   get_param_function(character varying)    FUNCTION     .  CREATE FUNCTION public.get_param_function(function_name character varying) RETURNS TABLE(routine_name information_schema.sql_identifier, parameter_name information_schema.sql_identifier, data_type information_schema.character_data, oridinal_position information_schema.cardinal_number)
    LANGUAGE plpgsql
    AS $$
-- DECLARE function_name varchar(255);
BEGIN
--   	SELECT function INTO function_name
--     FROM public."PosApi"
--     WHERE api = api_name;

    RETURN QUERY

  	SELECT routines.routine_name,
  	       parameters.parameter_name,
  	       parameters.data_type,
  	       parameters.ordinal_position
		FROM information_schema.routines
			LEFT JOIN information_schema.parameters ON routines.specific_name = parameters.specific_name
		WHERE
					routines.specific_catalog = current_database() AND
					routines.specific_schema = 'public' AND
					routines.routine_name iLIKE function_name AND
		      parameters.parameter_mode = 'IN'
		ORDER BY routines.specific_name, routines.routine_name, parameters.ordinal_position;

END
$$;
 J   DROP FUNCTION public.get_param_function(function_name character varying);
       public       postgres    false            �            1259    64910    ss_portfolio    TABLE     "  CREATE TABLE public.ss_portfolio (
    ss_portfolio_id integer NOT NULL,
    name character varying(100) NOT NULL,
    short_name character varying(60) NOT NULL,
    reference_no character varying(60) NOT NULL,
    address character varying(255) NOT NULL,
    city character varying(60),
    post_cd character varying(10) NOT NULL,
    phone_no character varying(60),
    fax_no character varying(60),
    website character varying(100),
    rounding_factor integer NOT NULL,
    remarks text,
    picture_file_name character varying(60),
    reference_file_name character varying(255) NOT NULL,
    user_input character varying(10) NOT NULL,
    user_edit character varying(10) NOT NULL,
    time_input timestamp(0) without time zone NOT NULL,
    time_edit timestamp(0) without time zone NOT NULL
);
     DROP TABLE public.ss_portfolio;
       public         postgres    false            �            1259    64916     cm_portfolio_cm_portfolio_id_seq    SEQUENCE     �   CREATE SEQUENCE public.cm_portfolio_cm_portfolio_id_seq
    AS integer
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;
 7   DROP SEQUENCE public.cm_portfolio_cm_portfolio_id_seq;
       public       postgres    false    196            �           0    0     cm_portfolio_cm_portfolio_id_seq    SEQUENCE OWNED BY     e   ALTER SEQUENCE public.cm_portfolio_cm_portfolio_id_seq OWNED BY public.ss_portfolio.ss_portfolio_id;
            public       postgres    false    197            �            1259    64918    ss_subportfolio    TABLE     C  CREATE TABLE public.ss_subportfolio (
    ss_subportfolio_id integer NOT NULL,
    ss_portfolio_id integer NOT NULL,
    name character varying(100) NOT NULL,
    short_name character varying(20) NOT NULL,
    subportfolio_start timestamp(0) without time zone,
    internal_contact_id character varying(20),
    reference_no character varying(60),
    address character varying(255) NOT NULL,
    city character varying(60),
    post_cd character varying(10),
    phone_no character varying(60),
    fax_no character varying(60),
    tax_address character varying(255),
    tax_city character varying(60),
    tax_post_cd character varying(10),
    tax_registration_no character varying(30),
    tax_registration_date timestamp(0) without time zone,
    tax_reference_no character varying(60),
    standard_tax_running_cd character varying(5),
    common_tax_running_cd character varying(5),
    ar_withholding_tax_running_cd character varying(5),
    ap_withholding_deduction_running_cd character varying(5),
    ap_vat_deduction_running_cd character varying(5),
    default_vat_charges_assignment character varying(1),
    hold_withholding character varying(1),
    hold_vat character varying(1),
    remarks text,
    picture_file_name character varying(60),
    reference_file_name character varying(255),
    website character varying(60),
    email character varying(60),
    url_picture_map character varying(255),
    map_file_name character varying(100),
    ref_map_file_name character varying(100),
    coordinate character varying(1000),
    latitude character varying(100),
    longitude character varying(100),
    user_input character varying(10) NOT NULL,
    user_edit character varying(10) NOT NULL,
    time_input timestamp(0) without time zone DEFAULT now() NOT NULL,
    time_edit timestamp(0) without time zone DEFAULT now() NOT NULL
);
 #   DROP TABLE public.ss_subportfolio;
       public         postgres    false            �            1259    64926 *   cm_subportfolio_new_cm_subportfolio_id_seq    SEQUENCE     �   CREATE SEQUENCE public.cm_subportfolio_new_cm_subportfolio_id_seq
    AS integer
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;
 A   DROP SEQUENCE public.cm_subportfolio_new_cm_subportfolio_id_seq;
       public       postgres    false    198            �           0    0 *   cm_subportfolio_new_cm_subportfolio_id_seq    SEQUENCE OWNED BY     u   ALTER SEQUENCE public.cm_subportfolio_new_cm_subportfolio_id_seq OWNED BY public.ss_subportfolio.ss_subportfolio_id;
            public       postgres    false    199            �            1259    64928    ss_dashboard_group    TABLE     �  CREATE TABLE public.ss_dashboard_group (
    ss_dashboard_group_id integer NOT NULL,
    ss_portfolio_id integer NOT NULL,
    ss_group_id integer NOT NULL,
    ss_master_dashboard_id integer NOT NULL,
    user_input character varying(20) NOT NULL,
    user_edit character varying(20) NOT NULL,
    time_input timestamp(0) without time zone DEFAULT now() NOT NULL,
    time_edit timestamp(0) without time zone DEFAULT now() NOT NULL
);
 &   DROP TABLE public.ss_dashboard_group;
       public         postgres    false            �            1259    64933 ,   ss_dashboard_group_ss_dashboard_group_id_seq    SEQUENCE     �   CREATE SEQUENCE public.ss_dashboard_group_ss_dashboard_group_id_seq
    AS integer
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;
 C   DROP SEQUENCE public.ss_dashboard_group_ss_dashboard_group_id_seq;
       public       postgres    false    200            �           0    0 ,   ss_dashboard_group_ss_dashboard_group_id_seq    SEQUENCE OWNED BY     }   ALTER SEQUENCE public.ss_dashboard_group_ss_dashboard_group_id_seq OWNED BY public.ss_dashboard_group.ss_dashboard_group_id;
            public       postgres    false    201            �            1259    64935    ss_group    TABLE     �  CREATE TABLE public.ss_group (
    ss_group_id integer NOT NULL,
    ss_portfolio_id integer NOT NULL,
    descs character varying(150) NOT NULL,
    short_descs character varying(60) NOT NULL,
    user_type character varying(1) NOT NULL,
    user_input character varying(10) NOT NULL,
    user_edit character varying(10) NOT NULL,
    time_input timestamp(0) without time zone DEFAULT now() NOT NULL,
    time_edit timestamp(0) without time zone DEFAULT now() NOT NULL
);
    DROP TABLE public.ss_group;
       public         postgres    false            �            1259    64940    ss_group_new_ss_group_id_seq    SEQUENCE     �   CREATE SEQUENCE public.ss_group_new_ss_group_id_seq
    AS integer
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;
 3   DROP SEQUENCE public.ss_group_new_ss_group_id_seq;
       public       postgres    false    202            �           0    0    ss_group_new_ss_group_id_seq    SEQUENCE OWNED BY     Y   ALTER SEQUENCE public.ss_group_new_ss_group_id_seq OWNED BY public.ss_group.ss_group_id;
            public       postgres    false    203            �            1259    64942    ss_menu    TABLE     B  CREATE TABLE public.ss_menu (
    ss_menu_id integer NOT NULL,
    title character varying(100) NOT NULL,
    menu_url character varying(100) NOT NULL,
    menu_type character varying(1) NOT NULL,
    parent_menu_id integer DEFAULT 0 NOT NULL,
    icon_class character varying(60),
    order_seq integer,
    ss_module_id integer,
    user_input character varying(20) NOT NULL,
    user_edit character varying(20) NOT NULL,
    time_input timestamp(0) without time zone DEFAULT now(),
    time_edit timestamp(0) without time zone DEFAULT now() NOT NULL,
    level_no integer
);
    DROP TABLE public.ss_menu;
       public         postgres    false            �            1259    64948    ss_menu_dashboard    TABLE     �  CREATE TABLE public.ss_menu_dashboard (
    ss_master_dashboard_id integer NOT NULL,
    menu_url character varying(100) NOT NULL,
    title character varying(150) NOT NULL,
    short_title character varying(60) NOT NULL,
    order_seq integer NOT NULL,
    user_input character varying(20) NOT NULL,
    user_edit character varying(20) NOT NULL,
    time_input timestamp(0) without time zone DEFAULT now(),
    time_edit timestamp(0) without time zone DEFAULT now() NOT NULL
);
 %   DROP TABLE public.ss_menu_dashboard;
       public         postgres    false            �            1259    64953 ,   ss_menu_dashboard_ss_master_dashboard_id_seq    SEQUENCE     �   CREATE SEQUENCE public.ss_menu_dashboard_ss_master_dashboard_id_seq
    AS integer
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;
 C   DROP SEQUENCE public.ss_menu_dashboard_ss_master_dashboard_id_seq;
       public       postgres    false    205            �           0    0 ,   ss_menu_dashboard_ss_master_dashboard_id_seq    SEQUENCE OWNED BY     }   ALTER SEQUENCE public.ss_menu_dashboard_ss_master_dashboard_id_seq OWNED BY public.ss_menu_dashboard.ss_master_dashboard_id;
            public       postgres    false    206            �            1259    64955    ss_menu_group    TABLE     �  CREATE TABLE public.ss_menu_group (
    ss_menu_group_id integer NOT NULL,
    ss_portfolio_id integer NOT NULL,
    ss_menu_id integer NOT NULL,
    ss_group_id integer NOT NULL,
    add_status boolean DEFAULT false NOT NULL,
    edit_status boolean DEFAULT false NOT NULL,
    delete_status boolean DEFAULT false NOT NULL,
    view_status boolean DEFAULT false NOT NULL,
    post_status boolean DEFAULT false NOT NULL,
    user_input character varying(20) NOT NULL,
    user_edit character varying(20) NOT NULL,
    time_input timestamp(0) without time zone DEFAULT now() NOT NULL,
    time_edit timestamp(0) without time zone DEFAULT now() NOT NULL
);
 !   DROP TABLE public.ss_menu_group;
       public         postgres    false            �            1259    64965 &   ss_menu_group_new_ss_menu_group_id_seq    SEQUENCE     �   CREATE SEQUENCE public.ss_menu_group_new_ss_menu_group_id_seq
    AS integer
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;
 =   DROP SEQUENCE public.ss_menu_group_new_ss_menu_group_id_seq;
       public       postgres    false    207            �           0    0 &   ss_menu_group_new_ss_menu_group_id_seq    SEQUENCE OWNED BY     m   ALTER SEQUENCE public.ss_menu_group_new_ss_menu_group_id_seq OWNED BY public.ss_menu_group.ss_menu_group_id;
            public       postgres    false    208            �            1259    64967    ss_menu_ss_menu_id_seq    SEQUENCE     �   CREATE SEQUENCE public.ss_menu_ss_menu_id_seq
    AS integer
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;
 -   DROP SEQUENCE public.ss_menu_ss_menu_id_seq;
       public       postgres    false    204            �           0    0    ss_menu_ss_menu_id_seq    SEQUENCE OWNED BY     Q   ALTER SEQUENCE public.ss_menu_ss_menu_id_seq OWNED BY public.ss_menu.ss_menu_id;
            public       postgres    false    209            �            1259    64969 	   ss_module    TABLE     l  CREATE TABLE public.ss_module (
    ss_module_id integer NOT NULL,
    descs character varying(150) NOT NULL,
    short_descs character varying(60) NOT NULL,
    user_input character varying(10) NOT NULL,
    user_edit character varying(10) NOT NULL,
    time_input timestamp(0) without time zone NOT NULL,
    time_edit timestamp(0) without time zone NOT NULL
);
    DROP TABLE public.ss_module;
       public         postgres    false            �            1259    64972    ss_module_ss_module_id_seq    SEQUENCE     �   CREATE SEQUENCE public.ss_module_ss_module_id_seq
    AS integer
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;
 1   DROP SEQUENCE public.ss_module_ss_module_id_seq;
       public       postgres    false    210            �           0    0    ss_module_ss_module_id_seq    SEQUENCE OWNED BY     Y   ALTER SEQUENCE public.ss_module_ss_module_id_seq OWNED BY public.ss_module.ss_module_id;
            public       postgres    false    211            �            1259    64974    ss_option_db    TABLE     �  CREATE TABLE public.ss_option_db (
    ss_option_db_id integer NOT NULL,
    option_url character varying(60) NOT NULL,
    method_api character varying(100) NOT NULL,
    sp character varying(100) NOT NULL,
    line_no integer,
    table_name character varying(60),
    user_input character varying(10) NOT NULL,
    user_edit character varying(10) NOT NULL,
    time_input timestamp(0) without time zone DEFAULT now() NOT NULL,
    time_edit timestamp(0) without time zone DEFAULT now() NOT NULL
);
     DROP TABLE public.ss_option_db;
       public         postgres    false            �            1259    64979     ss_option_db_ss_option_db_id_seq    SEQUENCE     �   CREATE SEQUENCE public.ss_option_db_ss_option_db_id_seq
    AS integer
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;
 7   DROP SEQUENCE public.ss_option_db_ss_option_db_id_seq;
       public       postgres    false    212            �           0    0     ss_option_db_ss_option_db_id_seq    SEQUENCE OWNED BY     e   ALTER SEQUENCE public.ss_option_db_ss_option_db_id_seq OWNED BY public.ss_option_db.ss_option_db_id;
            public       postgres    false    213            �            1259    64981    ss_option_function    TABLE     �  CREATE TABLE public.ss_option_function (
    ss_option_function_id integer NOT NULL,
    option_function_cd character varying(100) NOT NULL,
    module_cd character varying(10) NOT NULL,
    sp_name character varying(100) NOT NULL,
    sp_param character varying(500),
    user_input character varying(10) NOT NULL,
    user_edit character varying(10) NOT NULL,
    time_input timestamp(0) without time zone NOT NULL,
    time_edit timestamp(0) without time zone NOT NULL
);
 &   DROP TABLE public.ss_option_function;
       public         postgres    false            �            1259    64987 ,   ss_option_function_ss_option_function_id_seq    SEQUENCE     �   CREATE SEQUENCE public.ss_option_function_ss_option_function_id_seq
    AS integer
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;
 C   DROP SEQUENCE public.ss_option_function_ss_option_function_id_seq;
       public       postgres    false    214            �           0    0 ,   ss_option_function_ss_option_function_id_seq    SEQUENCE OWNED BY     }   ALTER SEQUENCE public.ss_option_function_ss_option_function_id_seq OWNED BY public.ss_option_function.ss_option_function_id;
            public       postgres    false    215            �            1259    64989    ss_option_lookup    TABLE     5  CREATE TABLE public.ss_option_lookup (
    ss_option_lookup_id integer NOT NULL,
    option_lookup_cd character varying(100) NOT NULL,
    column_db character varying(100) NOT NULL,
    view_name character varying(100) NOT NULL,
    source_field character varying(255),
    source_where character varying(4000),
    display_lookup character varying(100),
    is_lookup_list boolean,
    is_asyn boolean,
    string_query character varying(2000),
    user_input character varying(10) NOT NULL,
    user_edit character varying(10) NOT NULL,
    time_input timestamp(0) without time zone NOT NULL,
    time_edit timestamp(0) without time zone NOT NULL,
    master_url character varying(255),
    lookup_db_descs character varying(60),
    lookup_db_parameter character varying(60),
    lookup_table character varying(60)
);
 $   DROP TABLE public.ss_option_lookup;
       public         postgres    false            �            1259    64995 (   ss_option_lookup_ss_option_lookup_id_seq    SEQUENCE     �   CREATE SEQUENCE public.ss_option_lookup_ss_option_lookup_id_seq
    AS integer
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;
 ?   DROP SEQUENCE public.ss_option_lookup_ss_option_lookup_id_seq;
       public       postgres    false    216            �           0    0 (   ss_option_lookup_ss_option_lookup_id_seq    SEQUENCE OWNED BY     u   ALTER SEQUENCE public.ss_option_lookup_ss_option_lookup_id_seq OWNED BY public.ss_option_lookup.ss_option_lookup_id;
            public       postgres    false    217            �            1259    64997    ss_user    TABLE     z  CREATE TABLE public.ss_user (
    ss_user_id integer NOT NULL,
    user_id character varying(20) NOT NULL,
    ss_group_id integer,
    user_name character varying(60) NOT NULL,
    password character varying(60) NOT NULL,
    email character varying(60) NOT NULL,
    user_level character varying(1) NOT NULL,
    expired_date timestamp(0) without time zone,
    is_inactive character varying(1) NOT NULL,
    job_title character varying(60),
    hand_phone character varying(60),
    last_change_password timestamp(0) without time zone,
    default_language character varying(10),
    user_input character varying(20) NOT NULL,
    user_edit character varying(20) NOT NULL,
    portfolio_id integer NOT NULL,
    subportfolio_id integer NOT NULL,
    time_input timestamp(0) without time zone DEFAULT now() NOT NULL,
    time_edit timestamp(0) without time zone DEFAULT now() NOT NULL
);
    DROP TABLE public.ss_user;
       public         postgres    false            �            1259    65002    ss_user_favorite    TABLE     �  CREATE TABLE public.ss_user_favorite (
    ss_user_favorite_id integer NOT NULL,
    ss_portfolio_id integer NOT NULL,
    user_id character varying(20) NOT NULL,
    ss_menu_id integer NOT NULL,
    user_input character varying(20) NOT NULL,
    user_edit character varying(20) NOT NULL,
    time_input timestamp(0) without time zone DEFAULT now(),
    time_edit timestamp(0) without time zone DEFAULT now()
);
 $   DROP TABLE public.ss_user_favorite;
       public         postgres    false            �            1259    65007 ,   ss_user_favorite_new_ss_user_favorite_id_seq    SEQUENCE     �   CREATE SEQUENCE public.ss_user_favorite_new_ss_user_favorite_id_seq
    AS integer
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;
 C   DROP SEQUENCE public.ss_user_favorite_new_ss_user_favorite_id_seq;
       public       postgres    false    219            �           0    0 ,   ss_user_favorite_new_ss_user_favorite_id_seq    SEQUENCE OWNED BY     y   ALTER SEQUENCE public.ss_user_favorite_new_ss_user_favorite_id_seq OWNED BY public.ss_user_favorite.ss_user_favorite_id;
            public       postgres    false    220            �            1259    65009    ss_user_log    TABLE     A  CREATE TABLE public.ss_user_log (
    ss_user_log_id integer NOT NULL,
    user_id character varying(20) NOT NULL,
    ip_address character varying(60),
    login_date timestamp(0) without time zone,
    logout_date timestamp(0) without time zone,
    token character varying(1000) NOT NULL,
    is_fraud boolean DEFAULT true NOT NULL,
    captcha character varying(60),
    user_input character varying(20) NOT NULL,
    user_edit character varying(20) NOT NULL,
    time_input timestamp(0) without time zone NOT NULL,
    time_edit timestamp(0) without time zone NOT NULL
);
    DROP TABLE public.ss_user_log;
       public         postgres    false            �            1259    65016    ss_user_log_ss_user_log_id_seq    SEQUENCE     �   CREATE SEQUENCE public.ss_user_log_ss_user_log_id_seq
    AS integer
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;
 5   DROP SEQUENCE public.ss_user_log_ss_user_log_id_seq;
       public       postgres    false    221            �           0    0    ss_user_log_ss_user_log_id_seq    SEQUENCE OWNED BY     a   ALTER SEQUENCE public.ss_user_log_ss_user_log_id_seq OWNED BY public.ss_user_log.ss_user_log_id;
            public       postgres    false    222            �            1259    65018    ss_user_session    TABLE     �  CREATE TABLE public.ss_user_session (
    user_session_id integer NOT NULL,
    user_id character varying(10) NOT NULL,
    token character varying(1000) NOT NULL,
    last_login timestamp(0) without time zone,
    expire_on timestamp(0) without time zone,
    ip_address character varying(20),
    user_input character varying(20) NOT NULL,
    user_edit character varying(20) NOT NULL,
    time_input timestamp(0) without time zone NOT NULL,
    time_edit timestamp(0) without time zone NOT NULL
);
 #   DROP TABLE public.ss_user_session;
       public         postgres    false            �            1259    65024 #   ss_user_session_user_session_id_seq    SEQUENCE     �   CREATE SEQUENCE public.ss_user_session_user_session_id_seq
    AS integer
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;
 :   DROP SEQUENCE public.ss_user_session_user_session_id_seq;
       public       postgres    false    223            �           0    0 #   ss_user_session_user_session_id_seq    SEQUENCE OWNED BY     k   ALTER SEQUENCE public.ss_user_session_user_session_id_seq OWNED BY public.ss_user_session.user_session_id;
            public       postgres    false    224            �            1259    65026    ss_user_ss_user_id_seq    SEQUENCE     �   CREATE SEQUENCE public.ss_user_ss_user_id_seq
    AS integer
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;
 -   DROP SEQUENCE public.ss_user_ss_user_id_seq;
       public       postgres    false    218            �           0    0    ss_user_ss_user_id_seq    SEQUENCE OWNED BY     Q   ALTER SEQUENCE public.ss_user_ss_user_id_seq OWNED BY public.ss_user.ss_user_id;
            public       postgres    false    225            �            1259    65028    ss_user_subportfolio    TABLE     �  CREATE TABLE public.ss_user_subportfolio (
    ss_user_subportfolio_id integer NOT NULL,
    user_id character varying(20) NOT NULL,
    subportfolio_id integer NOT NULL,
    user_input character varying(10) NOT NULL,
    user_edit character varying(10) NOT NULL,
    time_input timestamp(0) without time zone DEFAULT now() NOT NULL,
    time_edit timestamp(0) without time zone DEFAULT now() NOT NULL
);
 (   DROP TABLE public.ss_user_subportfolio;
       public         postgres    false            �            1259    65033 0   ss_user_subportfolio_ss_user_subportfolio_id_seq    SEQUENCE     �   CREATE SEQUENCE public.ss_user_subportfolio_ss_user_subportfolio_id_seq
    AS integer
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;
 G   DROP SEQUENCE public.ss_user_subportfolio_ss_user_subportfolio_id_seq;
       public       postgres    false    226            �           0    0 0   ss_user_subportfolio_ss_user_subportfolio_id_seq    SEQUENCE OWNED BY     �   ALTER SEQUENCE public.ss_user_subportfolio_ss_user_subportfolio_id_seq OWNED BY public.ss_user_subportfolio.ss_user_subportfolio_id;
            public       postgres    false    227            �            1259    65035    vget_menu_group    VIEW     �  CREATE VIEW public.vget_menu_group AS
 WITH RECURSIVE cte(ss_menu_id, title, menu_url, menu_type, parent_menu_id, icon_class, order_seq, level, ipath) AS (
         SELECT sm.ss_menu_id,
            sm.title,
            sm.menu_url,
            sm.menu_type,
            sm.parent_menu_id,
            sm.icon_class,
            sm.order_seq,
            0 AS level,
            (row_number() OVER (PARTITION BY sm.parent_menu_id ORDER BY sm.order_seq))::character varying AS ipath
           FROM public.ss_menu sm
          WHERE (sm.parent_menu_id = 0)
        UNION ALL
         SELECT mn.ss_menu_id,
            mn.title,
            mn.menu_url,
            mn.menu_type,
            mn.parent_menu_id,
            mn.icon_class,
            mn.order_seq,
            (c.level + 1) AS level,
            (((c.ipath)::text || '.'::text) || ((row_number() OVER (PARTITION BY mn.parent_menu_id ORDER BY mn.order_seq))::character varying(100))::text) AS ipath2
           FROM (public.ss_menu mn
             JOIN cte c ON ((mn.parent_menu_id = c.ss_menu_id)))
        )
 SELECT ct.ss_menu_id,
    ct.title,
    ct.menu_url,
    ct.menu_type,
    ct.parent_menu_id,
    ct.icon_class,
    ct.order_seq,
    ct.level,
    ct.ipath,
    smg.add_status,
    smg.edit_status,
    smg.delete_status,
    smg.ss_portfolio_id,
    smg.ss_group_id
   FROM (cte ct
     JOIN public.ss_menu_group smg ON ((ct.ss_menu_id = smg.ss_menu_id)));
 "   DROP VIEW public.vget_menu_group;
       public       postgres    false    204    204    204    204    204    204    207    207    207    207    207    207    204            �
           2604    65040 (   ss_dashboard_group ss_dashboard_group_id    DEFAULT     �   ALTER TABLE ONLY public.ss_dashboard_group ALTER COLUMN ss_dashboard_group_id SET DEFAULT nextval('public.ss_dashboard_group_ss_dashboard_group_id_seq'::regclass);
 W   ALTER TABLE public.ss_dashboard_group ALTER COLUMN ss_dashboard_group_id DROP DEFAULT;
       public       postgres    false    201    200            �
           2604    65041    ss_group ss_group_id    DEFAULT     �   ALTER TABLE ONLY public.ss_group ALTER COLUMN ss_group_id SET DEFAULT nextval('public.ss_group_new_ss_group_id_seq'::regclass);
 C   ALTER TABLE public.ss_group ALTER COLUMN ss_group_id DROP DEFAULT;
       public       postgres    false    203    202            �
           2604    65042    ss_menu ss_menu_id    DEFAULT     x   ALTER TABLE ONLY public.ss_menu ALTER COLUMN ss_menu_id SET DEFAULT nextval('public.ss_menu_ss_menu_id_seq'::regclass);
 A   ALTER TABLE public.ss_menu ALTER COLUMN ss_menu_id DROP DEFAULT;
       public       postgres    false    209    204            �
           2604    65043 (   ss_menu_dashboard ss_master_dashboard_id    DEFAULT     �   ALTER TABLE ONLY public.ss_menu_dashboard ALTER COLUMN ss_master_dashboard_id SET DEFAULT nextval('public.ss_menu_dashboard_ss_master_dashboard_id_seq'::regclass);
 W   ALTER TABLE public.ss_menu_dashboard ALTER COLUMN ss_master_dashboard_id DROP DEFAULT;
       public       postgres    false    206    205            �
           2604    65044    ss_menu_group ss_menu_group_id    DEFAULT     �   ALTER TABLE ONLY public.ss_menu_group ALTER COLUMN ss_menu_group_id SET DEFAULT nextval('public.ss_menu_group_new_ss_menu_group_id_seq'::regclass);
 M   ALTER TABLE public.ss_menu_group ALTER COLUMN ss_menu_group_id DROP DEFAULT;
       public       postgres    false    208    207                        2604    65045    ss_module ss_module_id    DEFAULT     �   ALTER TABLE ONLY public.ss_module ALTER COLUMN ss_module_id SET DEFAULT nextval('public.ss_module_ss_module_id_seq'::regclass);
 E   ALTER TABLE public.ss_module ALTER COLUMN ss_module_id DROP DEFAULT;
       public       postgres    false    211    210                       2604    65046    ss_option_db ss_option_db_id    DEFAULT     �   ALTER TABLE ONLY public.ss_option_db ALTER COLUMN ss_option_db_id SET DEFAULT nextval('public.ss_option_db_ss_option_db_id_seq'::regclass);
 K   ALTER TABLE public.ss_option_db ALTER COLUMN ss_option_db_id DROP DEFAULT;
       public       postgres    false    213    212                       2604    65047 (   ss_option_function ss_option_function_id    DEFAULT     �   ALTER TABLE ONLY public.ss_option_function ALTER COLUMN ss_option_function_id SET DEFAULT nextval('public.ss_option_function_ss_option_function_id_seq'::regclass);
 W   ALTER TABLE public.ss_option_function ALTER COLUMN ss_option_function_id DROP DEFAULT;
       public       postgres    false    215    214                       2604    65048 $   ss_option_lookup ss_option_lookup_id    DEFAULT     �   ALTER TABLE ONLY public.ss_option_lookup ALTER COLUMN ss_option_lookup_id SET DEFAULT nextval('public.ss_option_lookup_ss_option_lookup_id_seq'::regclass);
 S   ALTER TABLE public.ss_option_lookup ALTER COLUMN ss_option_lookup_id DROP DEFAULT;
       public       postgres    false    217    216            �
           2604    65049    ss_portfolio ss_portfolio_id    DEFAULT     �   ALTER TABLE ONLY public.ss_portfolio ALTER COLUMN ss_portfolio_id SET DEFAULT nextval('public.cm_portfolio_cm_portfolio_id_seq'::regclass);
 K   ALTER TABLE public.ss_portfolio ALTER COLUMN ss_portfolio_id DROP DEFAULT;
       public       postgres    false    197    196            �
           2604    65050 "   ss_subportfolio ss_subportfolio_id    DEFAULT     �   ALTER TABLE ONLY public.ss_subportfolio ALTER COLUMN ss_subportfolio_id SET DEFAULT nextval('public.cm_subportfolio_new_cm_subportfolio_id_seq'::regclass);
 Q   ALTER TABLE public.ss_subportfolio ALTER COLUMN ss_subportfolio_id DROP DEFAULT;
       public       postgres    false    199    198                       2604    65051    ss_user ss_user_id    DEFAULT     x   ALTER TABLE ONLY public.ss_user ALTER COLUMN ss_user_id SET DEFAULT nextval('public.ss_user_ss_user_id_seq'::regclass);
 A   ALTER TABLE public.ss_user ALTER COLUMN ss_user_id DROP DEFAULT;
       public       postgres    false    225    218                       2604    65052 $   ss_user_favorite ss_user_favorite_id    DEFAULT     �   ALTER TABLE ONLY public.ss_user_favorite ALTER COLUMN ss_user_favorite_id SET DEFAULT nextval('public.ss_user_favorite_new_ss_user_favorite_id_seq'::regclass);
 S   ALTER TABLE public.ss_user_favorite ALTER COLUMN ss_user_favorite_id DROP DEFAULT;
       public       postgres    false    220    219                       2604    65053    ss_user_log ss_user_log_id    DEFAULT     �   ALTER TABLE ONLY public.ss_user_log ALTER COLUMN ss_user_log_id SET DEFAULT nextval('public.ss_user_log_ss_user_log_id_seq'::regclass);
 I   ALTER TABLE public.ss_user_log ALTER COLUMN ss_user_log_id DROP DEFAULT;
       public       postgres    false    222    221                       2604    65054    ss_user_session user_session_id    DEFAULT     �   ALTER TABLE ONLY public.ss_user_session ALTER COLUMN user_session_id SET DEFAULT nextval('public.ss_user_session_user_session_id_seq'::regclass);
 N   ALTER TABLE public.ss_user_session ALTER COLUMN user_session_id DROP DEFAULT;
       public       postgres    false    224    223                       2604    65055 ,   ss_user_subportfolio ss_user_subportfolio_id    DEFAULT     �   ALTER TABLE ONLY public.ss_user_subportfolio ALTER COLUMN ss_user_subportfolio_id SET DEFAULT nextval('public.ss_user_subportfolio_ss_user_subportfolio_id_seq'::regclass);
 [   ALTER TABLE public.ss_user_subportfolio ALTER COLUMN ss_user_subportfolio_id DROP DEFAULT;
       public       postgres    false    227    226            �          0    64928    ss_dashboard_group 
   TABLE DATA               �   COPY public.ss_dashboard_group (ss_dashboard_group_id, ss_portfolio_id, ss_group_id, ss_master_dashboard_id, user_input, user_edit, time_input, time_edit) FROM stdin;
    public       postgres    false    200   ��       �          0    64935    ss_group 
   TABLE DATA               �   COPY public.ss_group (ss_group_id, ss_portfolio_id, descs, short_descs, user_type, user_input, user_edit, time_input, time_edit) FROM stdin;
    public       postgres    false    202   ��       �          0    64942    ss_menu 
   TABLE DATA               �   COPY public.ss_menu (ss_menu_id, title, menu_url, menu_type, parent_menu_id, icon_class, order_seq, ss_module_id, user_input, user_edit, time_input, time_edit, level_no) FROM stdin;
    public       postgres    false    204   M�       �          0    64948    ss_menu_dashboard 
   TABLE DATA               �   COPY public.ss_menu_dashboard (ss_master_dashboard_id, menu_url, title, short_title, order_seq, user_input, user_edit, time_input, time_edit) FROM stdin;
    public       postgres    false    205   ��       �          0    64955    ss_menu_group 
   TABLE DATA               �   COPY public.ss_menu_group (ss_menu_group_id, ss_portfolio_id, ss_menu_id, ss_group_id, add_status, edit_status, delete_status, view_status, post_status, user_input, user_edit, time_input, time_edit) FROM stdin;
    public       postgres    false    207   ��       �          0    64969 	   ss_module 
   TABLE DATA               s   COPY public.ss_module (ss_module_id, descs, short_descs, user_input, user_edit, time_input, time_edit) FROM stdin;
    public       postgres    false    210   ��       �          0    64974    ss_option_db 
   TABLE DATA               �   COPY public.ss_option_db (ss_option_db_id, option_url, method_api, sp, line_no, table_name, user_input, user_edit, time_input, time_edit) FROM stdin;
    public       postgres    false    212   ��       �          0    64981    ss_option_function 
   TABLE DATA               �   COPY public.ss_option_function (ss_option_function_id, option_function_cd, module_cd, sp_name, sp_param, user_input, user_edit, time_input, time_edit) FROM stdin;
    public       postgres    false    214   �       �          0    64989    ss_option_lookup 
   TABLE DATA               (  COPY public.ss_option_lookup (ss_option_lookup_id, option_lookup_cd, column_db, view_name, source_field, source_where, display_lookup, is_lookup_list, is_asyn, string_query, user_input, user_edit, time_input, time_edit, master_url, lookup_db_descs, lookup_db_parameter, lookup_table) FROM stdin;
    public       postgres    false    216   �       �          0    64910    ss_portfolio 
   TABLE DATA               �   COPY public.ss_portfolio (ss_portfolio_id, name, short_name, reference_no, address, city, post_cd, phone_no, fax_no, website, rounding_factor, remarks, picture_file_name, reference_file_name, user_input, user_edit, time_input, time_edit) FROM stdin;
    public       postgres    false    196   ;�       �          0    64918    ss_subportfolio 
   TABLE DATA               �  COPY public.ss_subportfolio (ss_subportfolio_id, ss_portfolio_id, name, short_name, subportfolio_start, internal_contact_id, reference_no, address, city, post_cd, phone_no, fax_no, tax_address, tax_city, tax_post_cd, tax_registration_no, tax_registration_date, tax_reference_no, standard_tax_running_cd, common_tax_running_cd, ar_withholding_tax_running_cd, ap_withholding_deduction_running_cd, ap_vat_deduction_running_cd, default_vat_charges_assignment, hold_withholding, hold_vat, remarks, picture_file_name, reference_file_name, website, email, url_picture_map, map_file_name, ref_map_file_name, coordinate, latitude, longitude, user_input, user_edit, time_input, time_edit) FROM stdin;
    public       postgres    false    198   �       �          0    64997    ss_user 
   TABLE DATA                 COPY public.ss_user (ss_user_id, user_id, ss_group_id, user_name, password, email, user_level, expired_date, is_inactive, job_title, hand_phone, last_change_password, default_language, user_input, user_edit, portfolio_id, subportfolio_id, time_input, time_edit) FROM stdin;
    public       postgres    false    218   ��       �          0    65002    ss_user_favorite 
   TABLE DATA               �   COPY public.ss_user_favorite (ss_user_favorite_id, ss_portfolio_id, user_id, ss_menu_id, user_input, user_edit, time_input, time_edit) FROM stdin;
    public       postgres    false    219   h�       �          0    65009    ss_user_log 
   TABLE DATA               �   COPY public.ss_user_log (ss_user_log_id, user_id, ip_address, login_date, logout_date, token, is_fraud, captcha, user_input, user_edit, time_input, time_edit) FROM stdin;
    public       postgres    false    221   ��       �          0    65018    ss_user_session 
   TABLE DATA               �   COPY public.ss_user_session (user_session_id, user_id, token, last_login, expire_on, ip_address, user_input, user_edit, time_input, time_edit) FROM stdin;
    public       postgres    false    223   ��       �          0    65028    ss_user_subportfolio 
   TABLE DATA               �   COPY public.ss_user_subportfolio (ss_user_subportfolio_id, user_id, subportfolio_id, user_input, user_edit, time_input, time_edit) FROM stdin;
    public       postgres    false    226   �+      �           0    0     cm_portfolio_cm_portfolio_id_seq    SEQUENCE SET     N   SELECT pg_catalog.setval('public.cm_portfolio_cm_portfolio_id_seq', 2, true);
            public       postgres    false    197            �           0    0 *   cm_subportfolio_new_cm_subportfolio_id_seq    SEQUENCE SET     X   SELECT pg_catalog.setval('public.cm_subportfolio_new_cm_subportfolio_id_seq', 1, true);
            public       postgres    false    199            �           0    0 ,   ss_dashboard_group_ss_dashboard_group_id_seq    SEQUENCE SET     [   SELECT pg_catalog.setval('public.ss_dashboard_group_ss_dashboard_group_id_seq', 1, false);
            public       postgres    false    201            �           0    0    ss_group_new_ss_group_id_seq    SEQUENCE SET     K   SELECT pg_catalog.setval('public.ss_group_new_ss_group_id_seq', 1, false);
            public       postgres    false    203            �           0    0 ,   ss_menu_dashboard_ss_master_dashboard_id_seq    SEQUENCE SET     [   SELECT pg_catalog.setval('public.ss_menu_dashboard_ss_master_dashboard_id_seq', 1, false);
            public       postgres    false    206            �           0    0 &   ss_menu_group_new_ss_menu_group_id_seq    SEQUENCE SET     U   SELECT pg_catalog.setval('public.ss_menu_group_new_ss_menu_group_id_seq', 19, true);
            public       postgres    false    208            �           0    0    ss_menu_ss_menu_id_seq    SEQUENCE SET     E   SELECT pg_catalog.setval('public.ss_menu_ss_menu_id_seq', 30, true);
            public       postgres    false    209            �           0    0    ss_module_ss_module_id_seq    SEQUENCE SET     H   SELECT pg_catalog.setval('public.ss_module_ss_module_id_seq', 8, true);
            public       postgres    false    211            �           0    0     ss_option_db_ss_option_db_id_seq    SEQUENCE SET     N   SELECT pg_catalog.setval('public.ss_option_db_ss_option_db_id_seq', 1, true);
            public       postgres    false    213            �           0    0 ,   ss_option_function_ss_option_function_id_seq    SEQUENCE SET     [   SELECT pg_catalog.setval('public.ss_option_function_ss_option_function_id_seq', 1, false);
            public       postgres    false    215            �           0    0 (   ss_option_lookup_ss_option_lookup_id_seq    SEQUENCE SET     W   SELECT pg_catalog.setval('public.ss_option_lookup_ss_option_lookup_id_seq', 1, false);
            public       postgres    false    217            �           0    0 ,   ss_user_favorite_new_ss_user_favorite_id_seq    SEQUENCE SET     Z   SELECT pg_catalog.setval('public.ss_user_favorite_new_ss_user_favorite_id_seq', 4, true);
            public       postgres    false    220            �           0    0    ss_user_log_ss_user_log_id_seq    SEQUENCE SET     N   SELECT pg_catalog.setval('public.ss_user_log_ss_user_log_id_seq', 133, true);
            public       postgres    false    222            �           0    0 #   ss_user_session_user_session_id_seq    SEQUENCE SET     R   SELECT pg_catalog.setval('public.ss_user_session_user_session_id_seq', 68, true);
            public       postgres    false    224            �           0    0    ss_user_ss_user_id_seq    SEQUENCE SET     D   SELECT pg_catalog.setval('public.ss_user_ss_user_id_seq', 1, true);
            public       postgres    false    225            �           0    0 0   ss_user_subportfolio_ss_user_subportfolio_id_seq    SEQUENCE SET     _   SELECT pg_catalog.setval('public.ss_user_subportfolio_ss_user_subportfolio_id_seq', 1, false);
            public       postgres    false    227                       2606    65057    ss_portfolio cm_portfolio_pkey 
   CONSTRAINT     ^   ALTER TABLE ONLY public.ss_portfolio
    ADD CONSTRAINT cm_portfolio_pkey PRIMARY KEY (name);
 H   ALTER TABLE ONLY public.ss_portfolio DROP CONSTRAINT cm_portfolio_pkey;
       public         postgres    false    196                       2606    65059 :   ss_subportfolio cm_subportfolio_cm_new_subportfolio_id_key 
   CONSTRAINT     �   ALTER TABLE ONLY public.ss_subportfolio
    ADD CONSTRAINT cm_subportfolio_cm_new_subportfolio_id_key PRIMARY KEY (ss_subportfolio_id);
 d   ALTER TABLE ONLY public.ss_subportfolio DROP CONSTRAINT cm_subportfolio_cm_new_subportfolio_id_key;
       public         postgres    false    198                       2606    65061 *   ss_dashboard_group ss_dashboard_group_pkey 
   CONSTRAINT     �   ALTER TABLE ONLY public.ss_dashboard_group
    ADD CONSTRAINT ss_dashboard_group_pkey PRIMARY KEY (ss_portfolio_id, ss_group_id, ss_master_dashboard_id);
 T   ALTER TABLE ONLY public.ss_dashboard_group DROP CONSTRAINT ss_dashboard_group_pkey;
       public         postgres    false    200    200    200                       2606    65063    ss_group ss_group_new_pkey 
   CONSTRAINT     a   ALTER TABLE ONLY public.ss_group
    ADD CONSTRAINT ss_group_new_pkey PRIMARY KEY (ss_group_id);
 D   ALTER TABLE ONLY public.ss_group DROP CONSTRAINT ss_group_new_pkey;
       public         postgres    false    202                       2606    65065 *   ss_menu_dashboard ss_master_dashboard_pkey 
   CONSTRAINT     y   ALTER TABLE ONLY public.ss_menu_dashboard
    ADD CONSTRAINT ss_master_dashboard_pkey PRIMARY KEY (menu_url, order_seq);
 T   ALTER TABLE ONLY public.ss_menu_dashboard DROP CONSTRAINT ss_master_dashboard_pkey;
       public         postgres    false    205    205                       2606    65067 $   ss_menu_group ss_menu_group_new_pkey 
   CONSTRAINT     w   ALTER TABLE ONLY public.ss_menu_group
    ADD CONSTRAINT ss_menu_group_new_pkey PRIMARY KEY (ss_menu_id, ss_group_id);
 N   ALTER TABLE ONLY public.ss_menu_group DROP CONSTRAINT ss_menu_group_new_pkey;
       public         postgres    false    207    207                       2606    65069    ss_menu ss_menu_pkey 
   CONSTRAINT     Z   ALTER TABLE ONLY public.ss_menu
    ADD CONSTRAINT ss_menu_pkey PRIMARY KEY (ss_menu_id);
 >   ALTER TABLE ONLY public.ss_menu DROP CONSTRAINT ss_menu_pkey;
       public         postgres    false    204            !           2606    65071    ss_module ss_module_id_key 
   CONSTRAINT     b   ALTER TABLE ONLY public.ss_module
    ADD CONSTRAINT ss_module_id_key PRIMARY KEY (ss_module_id);
 D   ALTER TABLE ONLY public.ss_module DROP CONSTRAINT ss_module_id_key;
       public         postgres    false    210            #           2606    65073    ss_option_db ss_option_db_pkey 
   CONSTRAINT     i   ALTER TABLE ONLY public.ss_option_db
    ADD CONSTRAINT ss_option_db_pkey PRIMARY KEY (ss_option_db_id);
 H   ALTER TABLE ONLY public.ss_option_db DROP CONSTRAINT ss_option_db_pkey;
       public         postgres    false    212            %           2606    65075 <   ss_option_function ss_option_function_option_function_cd_key 
   CONSTRAINT     �   ALTER TABLE ONLY public.ss_option_function
    ADD CONSTRAINT ss_option_function_option_function_cd_key UNIQUE (option_function_cd);
 f   ALTER TABLE ONLY public.ss_option_function DROP CONSTRAINT ss_option_function_option_function_cd_key;
       public         postgres    false    214            '           2606    65077 ?   ss_option_function ss_option_function_ss_option_function_id_key 
   CONSTRAINT     �   ALTER TABLE ONLY public.ss_option_function
    ADD CONSTRAINT ss_option_function_ss_option_function_id_key PRIMARY KEY (ss_option_function_id);
 i   ALTER TABLE ONLY public.ss_option_function DROP CONSTRAINT ss_option_function_ss_option_function_id_key;
       public         postgres    false    214            )           2606    65079 9   ss_option_lookup ss_option_lookup_ss_option_lookup_id_key 
   CONSTRAINT     �   ALTER TABLE ONLY public.ss_option_lookup
    ADD CONSTRAINT ss_option_lookup_ss_option_lookup_id_key PRIMARY KEY (ss_option_lookup_id);
 c   ALTER TABLE ONLY public.ss_option_lookup DROP CONSTRAINT ss_option_lookup_ss_option_lookup_id_key;
       public         postgres    false    216            /           2606    65081 *   ss_user_favorite ss_user_favorite_new_pkey 
   CONSTRAINT     �   ALTER TABLE ONLY public.ss_user_favorite
    ADD CONSTRAINT ss_user_favorite_new_pkey PRIMARY KEY (ss_portfolio_id, user_id, ss_menu_id);
 T   ALTER TABLE ONLY public.ss_user_favorite DROP CONSTRAINT ss_user_favorite_new_pkey;
       public         postgres    false    219    219    219            1           2606    65083 *   ss_user_log ss_user_log_ss_user_log_id_key 
   CONSTRAINT     t   ALTER TABLE ONLY public.ss_user_log
    ADD CONSTRAINT ss_user_log_ss_user_log_id_key PRIMARY KEY (ss_user_log_id);
 T   ALTER TABLE ONLY public.ss_user_log DROP CONSTRAINT ss_user_log_ss_user_log_id_key;
       public         postgres    false    221            3           2606    65085 6   ss_user_session ss_user_session_ss_user_session_id_key 
   CONSTRAINT     �   ALTER TABLE ONLY public.ss_user_session
    ADD CONSTRAINT ss_user_session_ss_user_session_id_key PRIMARY KEY (user_session_id);
 `   ALTER TABLE ONLY public.ss_user_session DROP CONSTRAINT ss_user_session_ss_user_session_id_key;
       public         postgres    false    223            +           2606    65087    ss_user ss_user_ss_user_id_key 
   CONSTRAINT     d   ALTER TABLE ONLY public.ss_user
    ADD CONSTRAINT ss_user_ss_user_id_key PRIMARY KEY (ss_user_id);
 H   ALTER TABLE ONLY public.ss_user DROP CONSTRAINT ss_user_ss_user_id_key;
       public         postgres    false    218            5           2606    65089 .   ss_user_subportfolio ss_user_subportfolio_pkey 
   CONSTRAINT     �   ALTER TABLE ONLY public.ss_user_subportfolio
    ADD CONSTRAINT ss_user_subportfolio_pkey PRIMARY KEY (user_id, subportfolio_id);
 X   ALTER TABLE ONLY public.ss_user_subportfolio DROP CONSTRAINT ss_user_subportfolio_pkey;
       public         postgres    false    226    226            -           2606    65091    ss_user ss_user_user_id_key 
   CONSTRAINT     Y   ALTER TABLE ONLY public.ss_user
    ADD CONSTRAINT ss_user_user_id_key UNIQUE (user_id);
 E   ALTER TABLE ONLY public.ss_user DROP CONSTRAINT ss_user_user_id_key;
       public         postgres    false    218            �      x������ � �      �   >   x�3�4�v�.-H-RpL������t��FF�ƺ&
�V�V���ĸb���� ���      �   {  x����n�0E�3_A��4|ɖ�3�4��q
'��bR�6%H4��}G�T�(#�Y�����k����?�	6�������$����1���F,�L`���c}�^z���0E��e�	\���e���&E5e�� (8������KSM���Q��K�׃��m��y����2��VJd��43���koA5�R��Tמ��hl�~~N���-��}r4���O�:=fo;�j�h/&PB��k��l���<�p	߳�KV����l�;��~o+v�|�
��=O5�-yg[\�q�-S�M������ڙ�Ul�2�*[��֤��Y)��a
p���9?dy������̷a
E���|t��RnH�G�������f(V����*hlѼma�(�-C�ҶLmٛ�&���ak�	ٞ|��R(�]�:\��W�a�R�x̪�z�s6�1;C��ԱfѬ��{��5M�eH�J�߿�L��ʿ����^�B�1��ȭ���ʼ*�����=O��Pӹ<������}ήrV_&� �9�4\3�+���eU=ۿ�o�><�>:��#9���q�5��ӓ�Í�WԲ�K��Of�)�+x(��۱NN�4�n�*��e��Ꞧ����f���x)Ϊ      �      x������ � �      �   �   x���M
1�9��K�;/�lf#x�IQA�`!t��#|���~��s�����^8,���-pKq��� n%��JT!���Br+Y��V�
ŭT�[Y�-�ŀ�������ڻ�0�n�gX؆Y��(#Xq������ #Xi�)!�2�������� �F�$���"z�Q�      �   �   x�}��N�0���S���6M��"��B���$.�PD�M%x{�	$`Q.9|�?�%���DS�)}��{�N'���� ���J4+�VR�Z���0�5�0�Q��
��Gq}��+m�ʰ560�i)������_,�Jf�FK�4�D���J����I.��6#Q�\����l\I՛V�j�-qE���.ǰ�-񲗱��c�3lm�ss�+��a���ܮn�\>�!���/      �   L   x�3���x�ԼR�� �Wδ���\ ?�8��$��Ӏ�3��/�J��*X�61�=... Og      �      x������ � �      �      x������ � �      �   �   x�m��
�0D��W���{Mc�]+�o���Mlb-h���"�p�3�A�����w|ܺs�o�e��fE@lr|�|eo����6�#�LK�
F�V��]0|�9�����5;t�%�	Ա���2O�E��f�b$���W/s��ѻ�V��ic�h�*B��W;o��tQ.?$ ���H	�3��`+� x f�A�      �   �   x�uO�
�@=�_���$S��R\�^/��"�)���כ��M^^H �ò�qX:pb��,|�&M�Ed�	 /�V� ��V�up�W޻}8Ug�hqYڻ��x2-��IZ=�4����#ɼ�`kT�p��6l�?W�/����5��O��Y'��E#�)�ш/oh ��7����M6/o�~�RNR6�z�6���5BM      �   s   x�3�,N�4�.-H-RpL����,�r����IIs�w	5��	q���,)�K)rH�M���K���t����p���%�����@��X��D���������W� �%2      �   ?   x�3�4�,N�4@dd`d�k`�kh�`hfedjeh�M����S�����	a3����� ,y4      �      x���ْ�J�����b�c]�Ҭ/�Ab3�o�<	��U��w�����KYf�o�����;¿=���`
���E~C �����������o������>~}�}˿m$qSfJ����zO��B���?�u���svǿh��~'�=��}�cr�,���N�P�B���(�y�5�t�X<�Ħ����1T����B<�]̡�X�G�Z�-���kS�p)� ��`���=� �0��֦ME#9R�/xe�9�n+����8�+��L6��`�ܠl�`^��J���֢Z���2����W[�]�)�,�K�����y>Ѿ����}@�)-�L���)x�﻾&���EJy�� �b��7��]$�9����v���it���*��+��˄F6�"�|�7���r�ujӂx�[F���&H#���n�Ҫ�wW�B�z<�-<��{rF��!e��t���]̈�y'w��D̯֜i��|�}��oUU_���X!��P���m_Tdu��Q��s=B��]�:�۟5�����0�w+��ð�!p�������3��$�2{�>-�ih�6#�
(��*"�3�4���2q�c��\�ǳƝ9P5 �K��_����C��Un���p�\!s�lGmsl���-8��F�jѾ�u�wK7�T���vR��G�Lt|��"�w�ƴ�<�9���|�r��|�}�/�u��0��?P�\��;���齐,~���K�c�ެk�Rl��}�k��9�Z�>����F�b*�6��i��kH4YY����o1J�8�'A�>��漋���u-��e68x��M�,��'�봰U�>p@�͕�d���y����F?��ؕ���+\��|�}�������Ds�K.��kw�ى��]��x|�G��]R
A��p߭�JR��ߪ�1�x���D<[��]c��h������ē�!��GN#������XM�y�br�*U��aL�J���������z�~i�VR��!w0ܴJ�R	er:Mq/2Y�W����D��W�`�g�����|7J���޴�9[:�6KE*��x-Z[���&�lx��a�*/í˺���pQ�T����]��Y�[�=\
��K��FT�F� �x�Q�)�U��COd�ɴ�k���]������+O��>| ��oZSi�I��zmx��!��6�+g�>{>�^��u���Oi�+ѻ��B�ή$�Ɯ$�ꙁ�Z4&��2�:b����ɭ�i���L�u���h�m4���n���,fl,V�*~E��5�YU4g�j*.�#���i�]��o�S��#��Y�g�[��Oc��a�u�,f�=L��m���n� �k^�=�h������������<����B���v�W����OI`���6eE/��`����4�\�̊���D�-��(�`A=B���W	���ue^��3Vҫy��$ڒ��20ZI������	��s�Ԕs��¿Q��''WR�U�Zj���B�>9�,�,�����!K���퀅���B��<yU�������i�����OI�ڋ骬��{��t�\4ȼf�ە������ �-��v��+��m^��a��	?Y��x6�Bs>��`tŞ�t_r�b��4�c
,襤����J�C�UX�m( V�����Z-�mC����������u�|��������2 �LW1h�lkA�]Q�ex�Ǩ~:n5S+��)d��
d��^n�o��j���W`k���5(������a�۴ն&��q��U()��f;J �4~��`�e�~��-�fL�� ���EMf�F����VCSN�;Eπ��x�p�^��|���ZK��Nyuu���%���s.˝Qu:"+�����A����H%�(֩������8���Np(M�>����#5�^b�T���@��4XS4U�T�Xu��є��#uQ����*�z�^��u��o�Ѧ��u�$������d���r��.{��)�OK�ϞO����M��q0����3�@�h��a�#��1��NJhɓk�����Y�_�&��,�R��Hb��k�Y��y���м����і�Q�:N0�*'w�e.+��E�����@L���d>a�vt��!P��y�}d�����d뤖r1��9�@�	X�1�%_�=�h������P�>��G,<.�/��#s/�|�?u��+Ex��QQ2�ے�����<\(�ū7T���Ÿ����s�4�;Ei'���o�%1�3�V �Ś�$@my��Z�HW\���TɆ�p�����^��C�.�mIQF�Bs4|_�൵���<��'#�k^�=�h�a��S^���0qt�G�vڕ��~_����U'�1'�e?�?��c:��d�"R� z��V$��X1}�����"ְF�	^�P�'���š�� �^�7(�=���RӢ�zc���.��]D��R�M���}A���W�)s��猕7��z��Ed�7U��W����`��1�t�	�& �4pE�`'���x���}�|�����F��y����_���k���;���8����5��L;��k\�̈��?g��OW�h�l���l���2u����=���8��V�w��󣒸�Z� S����h5��&������gG���~N�e�N��S	��E��;�nWnxO�gJ�
���3=��t>�A�����r�欬v�-�Ȓ'J^��p|)�hZ�#;ZE�#��yT��3�'�'���mǧU�����":�A*"�!¥��vV�p͕p��5���)�: |����A`o�H�,8 ���d�rهxp��^j�e����B�&�`�@xt�J�I)��~O�RZ ��P:X�vo�E�����Ne�7d���� �g����9��ķg���i��jw�6_Oy}�|��~�0�����P?H��ȟg<����]7��ʃ�V�HO�6>EU�56��uj�A�R)���]r(M�[DVl���Q�&�.�7w��[sT�^��b���rZiCϬ��8R,���\I��E��0��@-�Qb����O���=+!��`�!H��~���_�Y8�s�<�h�?�$�Aɿ�$�z��
<jMP ����R��XXb2SB|���,�qa%�W��Yt�v�q��U���E����t=B(�1��#J�S�c �j�ȆX~m��9:�8� ��-��u���G'"�C�-����,�!�4h(F]�3�%l�i�w�eb��C�����v#�#���C�sf���0ٰ�( ���2-\�r/`�#�f���S�K�.�l�=�H���o�>��ty�*H�awl����yСO���W��xT'�M���=�*u)�����`'%a��/*�J�[�R=�L��s��4F���TG�K\�ǻ�ޙ����kb�=�h���]	c��_�!�Q���L��BԃUw3�a@�I��բ4�B��Y�����#�`����T��j8z�AY1*�
	}�j�pY��V�l�M�ͬ�Yp��������_��~���c�rd8fD�2�gUN'ځ-|�q��ׯ@[]+	��9@�&���v�݁��"~�?�O��_o=��:\JS<~�L�%�.�����ET����fY(����h�2��:�#�,��c�� �E
F��J\���Oo	�qu$��� y�X-l��-��A{,�A�j�وJ�߾�[��u]�b=���E���a�$|!�W:��E۫��q�_��[Oy}�|��~w 0�F�0��?������O�,��|Yu���pA���7�fq �i11���7�cȻ����'$�j��5��)&L)��q+[:�@��O�^��'�/Խ�,�%&�<�Č��Z%o�����xׁ,�]��ٺ_�����<�2Dzn%!9 /V7��p?΃�1m� �<\A���Sb��;�B_���~��o�7\A��mh��bI�//�^�0��B�� ���;Վ����<1�I����Tbe$�?� g��t����L"��煭\�^����+ ���2���%� $  >���C�vk��-����4 WV��PMj1U���J�WY�=�B���;+e���˘O�\���D;`�*�?>��	�?0�:�%$�ye��upH]�iG����Feĵ@��c�)�1�������VW:��%��2�6í��=k�D`�0�|�0��;� �,HL_��@^-�֔����Vˣ� 01�H��EU�0��� X��s�o�&r�M<\�Q~��/���߉v AΑ�Ǟ$N��u:ԓ���˒u�g���ԧ�g��u	�)���8 u���)�i{���A��x�V�R%r�P��[��4���h�"�U鴕P@u��0�l�Y�a����:�r^!�8ӿ���q\�W�!��ۃ�C����N�	z���W_	}��`x}}�|̾���Ķ'��q��w I=� );vL�ʡ"7�>$�ֹ�7�-��=Om�3�U7s���t���ֱ>������Z\漏e0�<���F͉=8�b��=`
{���Q)<k=��hȩ��1���G)A#��zC�(_��fv8u������������5[9�      �      x���ǎ�X������O��w	��H�V����{+:��*rW��ܕ�F	(��3��\k�I1�/�bҮ�����<�í�'��ie�c���(H9QID�����B����:6;�5OL�±�9C��s�6�P���@��T�[}��&�@�y����Zu&��*��=&v�v���A�Ā����Q�`UMh�˃6�K̀�0���.��+<]X-����/��_�?0��~����b����]�_����@���B̷h>�+Z��L1�c�fx��ǽ������J&���J�Q���DU��tc�͘#�i�ZpF�DP�{��"���W�Ad� ����8�w���d c*hJt��h@�i9J.r�\5����
ݜRP�S�� F�.nvPu�Ol��0�|/�����z1�?���C�bo�ߊ4���G�c�2H�zoLu|�K6kѬ���"����-`�:VĔ�0�]Eod�Kj®���̈́nC��-����q�ˋ �z�c	�+�ש�jZA˕i]��>��$)�3y�.��ciի�P?ɉ7�li�MP�uu���D1��f_�#@��b�_F�?���4� ������x����+L�]�f9O8��Rq9�Z �.���"�ץAH�8�&+U5=ci��J|q��?���f+����fU!��D툼�⬜\Ubr�[X����φ<P,0,���\�쨧vI�!���*e�1tE�9��)�;��2-�����M�~��w����lڿ^ჽ+�+W�'&Ə[N(wz2��LE�M��ή����ATc��.8y�2� �{�ջ%F�
��H��|p�$e�-���4Y&=���"LΙ�{v,Y�M��SɈ�I�u�,'�ػ|��,�ϝi�0��/E;��^OL֡
�x	e���YV��<����L���SӾ]ჽ�+H[�m��@�v�-�v4[[i(e8�R��ײz�X5]���dn�=o�aیJ�\ܶ�[�I��gBFAX��*�օP(��hJ[��ns���[a���B�ǅ���&!FK�"�#����t�R1;��i|����O=���qt��Z�.c�)9�\�GI�HQ��/W�L=����qď��?���3u��F������3y�>P��:��Q�����e�M&��],�rH
����8�G-F�P.:*3,��6�7k��Vi�����5xlt����b`�Z��MH�8�92�8�)��,����*ѵ��$T��G�v��@���q6D�1���{C�{��j*u�I�-�-���L�J|7�����̯W�`o��
�iZ\���.Z�1h;Ş��c�D�M͹Q���q�pJPO]\��[X���D9 �� �Ɋ� ������NՑ���{�*Gh����R��!�\�0��ʣ���\��[�x���*[`�n�ե�VB�����ҙ�ݠ�϶d���>�?��)Z��_��_i?��-)O�濎T4��Wp�f���;�e��:Ma,tJ���V��V�6��n=��P��P؈pٞ��֌��T^�=��T��x>�Gy~�^�缲"�bĔ	�7y�b"� �d-�i��h�e<�&�fI܄��"��%�o)3�~q�>�~���(���8�]k�/+��n�� �|��`�&�?�?p�7�~��{�?><��eW/��#�� �X�ex zM�B���谦�����ҳa� ��=_tΖ�('�����+��5ssy�,��iy�z��3�eSQ߈s�MvD�z�Ap�/���KA�lnWaI|�3v=����5E���-9e&��N�pu>�*�
���JԞ�7쟆���o��d�C�]䃽�ɹ��4}���Ld�fN��}���L&�j+]�{��գ~h�E�a�NN)WV*7c��E]��8;�t{
a�ݯ�s��Z�f�]aY��֛�/B��kOk�F=�b<-���o�	�i���e��e��F�t�(@c±��U>�) z���,f��Z�D��Cq��}��d�C�]䃽��_��g���.���L��(���j��5��V)+�z�橍rТB%R�(L$;���j��`h�X�6M�q;W,fho���ж�Tq��#���0R}�T��Mk!����gs!�`%�(7�����f�mw����nI�ى�-�#��3��E�lh��F���M�#y�����j(���ȟ��'C�f������{�?���-�!�SS�y�!)� ��RYطiMvZ[A�Ɔ�������Q">Q�j�V��8�J��R�5��cԙ=f�w�*ģ����U���<RE�t�	:&9�J�^i
my�cX �ɑ d�k�ˊc�:�ג4��݅�I�v�`4��:�cӽP�s������P�u�~7���o�z��2��-�稸5�&��$�r��v�lz*}�2Bw�
�ޣ�"�����5*�ܹlO[��j�k���SNk�'Kp�S��6I��� ���D!��*�<V'��U�=)�&��5s^󻼧jx���<2�i�
��;|�qIP��ɮzM�s\[�$��FU��@ �{������(B}7���o�z��HQ���P-U�k ��񮂽zi���d%�m5|Ҋ�_E��ZW`1x^y؇�ȩ$��B/u�<xr�ҫ���1��P���$U�f�����3�!�cD�9�h�W}&̬��G�|��3���z$v���JĎ��U�c6���Yv���$�:t1��C7]p���P�D���v�_�?6��`o�� dl��5��s�!I@E#�,j�	�G���+�ߍSy�{�����}�Y����`$'��ײ�$� YG�=��\��TJ���r�Q�:�qo6�O�}��L=�/`�	.s�(���|J�m�&T�6��&��+ .�㚬�qW��m�ӹm�EQ1j�iL���?����!?����o�`o0�_��n�5o�f��KՊ�p z�ȡ���MA5}N�j�v����峣�җ�"��6Н��P�#4EBKh��<C����j�u������fl]���ޕ{�#����l�Yk�q���3�X�W��K)z�u��pLi��cL@�h��+������?o�}��}��U��������3_�{�/N=��VB�m��NE,��5�V��)R�B�zkG��qm?�Вx�R{�W2O����&GPqt�k_�̹@�i��������9�4}�JE���[i���!��jTi�귇��U,�ψ2iZ����Ќ��=b���X���ᜌ��r�諒�	�ݩw�{��J?���n��tc���]nPvO'�����s'4��ˉ�Wl�����n�O��0�ǝ�o��z�<�;�Z*�;D�w�jj�8��ho3yV��'Sڕ#��) ���E��hp�S���E�ve �d�(nS��c��]A�������o���6�{O�d�u��!��&c�7ې+X_����.�o�3_GF>M�Ddl���9���	V����`��I\FW����{�]��ϦI1�X���#ԇ	E� �����uG������wJ*�x4��paa:�� �}�l$N+;.A�t/
�--0Ǿ!������4��+�}s��ީ����2���0����s�7#?EyIR���33;��AU#�1��L�v��"� � =[Y��.#�J��������wl_a��lO4�(h|��BD,�Uv�'DG[�1�+�*�)K@��BW�3�뮂���U���$c�ju��BmqUnq�������4���~X�d�u��!��f���e��Q�2�j	�썐7�]�"<�r���ZFm�֭7jς�.����H8WV�]�0���3�٥�:[%�(�Cѳ����KL��>��kI+�kOڮ�9��Y۬���=����95��k�0L�':�Xh�s���Fɕ���K#~�������N}�~�7{����NF<�5y��c��4S%����cK��;��d�u�g:S��� ��ū|+�x�L��5��	�'a���~���@���{l����>�.gȘ{-��%y    m�^��fW9�}��Ӡ�-�2�8a6K�����k���;K(�.J]|�8��:��K�z���`�w��Z���!����aS\�	f�w���3�-�.�&�"}�&P"�b��珐\$�M�G����HD�����8�����j��]�p�w�WD��qSLP#�Fl>VN������1���>�h|$#�w2�%v����YV�ߒ��^��Y�&H��rt�s�?K�GTE�U���P���.��ZLfj�t/.+��B��f�1�i=
!OD�PH��2��ϫU@�|G�<�*�ݽi�����q�-K9��J�I�~��L�ew#��[H[t��2'�j�@�fx�Қ`>SӾaf�.$^n��m'�+���d%>��f ~+�����S?�o��&�`o���ba��  �����C19ӳm�&������35}4�,2�\,el�x�
��uC��x>��^�����"�V/�p�Wl���)SH�2,g�$sq��S���@��u;i|��wUt�Fh�{H:��Q��"�A;NbX���=�A^���93�R�g ��Γ�?o�Ӡ(>��ͯvm�葷9;�)n�u�/g�m�K����vr�^��:�G2�K�9Ju��fӣ���Ỉ ��$>�\�`��
���W���� �<\I+E8�L�څ��Cw>��e$.t�}����{�=��cw%�<1�x���.cۥ���3�'�h%���Sߤ�y���x�NR(g��X�G��ゴ-��G���:�n,��¢Ĺ�sP �a�8V�ѹVg�_tLc^H>�ĜZ̐s´��v��:��l%GJ��0�c�3����EM����`��47h�.٭1;	�3�ZU+x�W��������WTW?J�߿'�?aC�S�w�~!�����h^�k���)���(���G%1�ޱE��mqd{z�@Zq�� ��	�ihM��x`c;Ȧ�>o��;�����(	� e^C.�p�M���[i�c�i=��I���t;B��+�>_�7���c��� _3��Je���%(����?s�ͩ��߶�_H?�7�}�jT�������=��� ����Ƌ{���=)1p�YUE����\Π�J��Tr�@y��Ȇ�?�'4L�Ì�Dk���$�{�JX5e����ZYM�j-�����u��Ĉ����Q?uHi5]m�p3��D���E:�p�3g�{i���濝zg�����O�}�愨���1'HN��`�S�@"����3Ґ�&j�bӢ�R�z�"b��Nۊ������Ǧ՞�w�@ՀL���r3ؒ[�D�.w�@8��n�P�lv�v8�`�ޕ<�ѓJw�0�軕F�jfy7+B���i&:=V���}c;J^���.�|.��L��٩�ٿ;��'���G�\�Jn�@]��9}������sh��d}9Ĥ߀	�$�-X޹h����� �#�W�[��Q�m��H9pt`��xH>r�n�;�	��a~<�C]Z�nE,��.���ौ���j��)�s��I��	_;�D�z�z�������8�����f����!�����hW�Q�^�o��Y�o������T�7��`�_+�Kj]w9�;:A�ոhVL����<��f+��g0p�-F��$���*γ푞9���#Ӱ��3Dm�d�p>}���b�^����ZF ���i��:�ǵ��Rz%�R�kW���S�;�~!�����h�+�#!����=��u���w�l�`+,p���OԻ�v��X,����I��*�����ݚ�Z���[��]R�t�j��C��+��|j49��5r<D�Qʥ���ae?5|��:�k�R~��;-<?[��0r��&�S������J��w�~����w�'�����1+��rϗ��9������+0��f Ɛ�n?�K�I���`Z�&�n�-�8�u)Ղ���Ġ	�,S&�VZJe�ıBPƮyTCW�TI�87�X`�� ��c��0�R7M	�&��#����2p{W)M ���H�C\G�p�7���4����ߜ��~��W�'���7��le[��R�lg"e��XҨ�Jόo�a�R7W�aN�_ǝ��kZ�Ĳn����8r�Xy����#�*n�/hf�I|"O��_��٠��{�V�sJi���S7�궕}bq9J�ܟz�l�ޤ�b�m��AY��Ug���_J��,��8���Sߥ���~��0m���ݐ
*?����&0�m��֓�^��*�Xc!m�"�2;.�3�&v���e�բ��k<��/p��:��$w+ם��L��yw^C9M�M[E �9/vP������/�����g�dуG"`E���Z�<�/�~w�]}��8�]�������MOg��<� �ꝁ��:jR���g�F�x9�;yG���%V_\��Ԁ/0��e�FЭ�le��ު�v�Rܐ�r�����+I����V��!&��B��N�ɻ�s��"a�h����PZ4��F4�ע�V���޹%vp�u��E�gg�o��Y�ͩ��N}�~bo�{F�'<�^��԰�l>Rx\��
_׭cw��N��?��n����]O���9����B@���_�=��G��bK�!l4rgr@�TF������b�p�u�n��z	�\ԕ<��ⴧ��?�����Y��ny�����n9Q�9�h��F����N���;�E���a�]wŞ=L]7:��j�࠵�J��)����)kl5��YA����}On�9?|?#���E�D�6�šW�ڪj+c5�7_Y8Ķ��h���ĘY�1M��]}y��H>Z�pr�c|l���ER}�;�u�f93��4��D��i���� ?0�S?�o��*��ް��O���1:�P��s��f��}�������Ϋ�;�1672����X���=��fxQ�+�gY$�y��y�{''����lw�Gt����T<�A cW����U�$��#�.D�qNuq!:%�!�j��Κc]������nS��uO��K�>?���N���;�E���a�=���	?�,|����ME��L�V�����!��&s1O9�'[y�ױ���:����kESTq�d�v݌�Y�y�$��Ц�žݳ�U�V]N���M��r�;����]�5�G�x�k�W�#�Bb�m]���<C#�U�o��L��}�n��Ҿ�K���ީ���bo�{F�K�r�ΰ'��7�`���?��RR*���n�֒":}�T��gɳ��^<�H�c+�k���^7?	o�@<��9x�5�K:)�*A�p��A@j67�J�Q/�T�� /���n�T*�[<�-'�^3��I��!w4ܴN�J������2Uܗ�^�
��~�N��~3�}�~bo�{F��ݯ���K����"�4�V��ZW���o�c����,a�<�^�G��!�@�.
%bɳ
�� +\
��K\(���A6���SZ-�%Շ�1*,�iMߦ�ݻ�����W���CX ���`Zse�I	��~m��!�Lۙ>z
���_�:�iBF>ؿ8�K�{�ޛ�J�.*�ƪhl'�m��*��#��Qc1;OS�$�y"�ey{(����+q�ϖ�EI'�����_S�L�����y�{N�3�yz��H�^�ѻw����KI^-���n��D���u����M՜����:�v�ϛm@I��]�o��_�O!�_V�/��{��
_�w#�)�Γ�)�y�Kz�gbkєRʮ��׮qg����Us6�QQ��uѲ�O�[g��|���X��x�Ck�)��h�\�TX�NЃ���v-;�����(��amo5G��㴨q�u�-v�=��;�9I+��6�J(�K#@�w���o{��a��P魔]�5$�;��X*��mU3k�!X�r�箭F=v)�"=�V���2`.>>�'X�E���q�U��gS�W�������I�%��T`t�tw5{�Y�ܹs�Ԕs��"<Q��g'WR��4�Zj���B�>5-Ϯ�����!k �  5�K���)�/���"~i�W�{��z=(*�3�Ŕ1-������й��ĩ����� ٹ�= ZǨU��kv�]��Pm�c��g�O�L��w�����0G.�8�"=�sEax�_��{��}�].���� �i�d��u�yx@��I8�Y��2
��{���V��X�����J{���C��~�Pߤ��.ǂ<2_�`��kDw/�,�G3E��q���@�^��dR #E���|�[m"�.N� [�֮�@y�߬�r��ܥ�v�� B�x\���)�'��"Ȧ�Gs�h�uR6��_0�6 ��$v����\O��������w�Y ��Z�������\(��S�w�����7a�4�⒑柼C�wV�]��M���O��FlO))�@�^-�N�2f�|{6���P��C,U��D4���֋�T �x�64C7.]9VSsB4�~�H D�
q����qp�=��C4:��n�l:/��&�݌�EgS�;$�k{��p0�\�S�Ei�/���3��_�7�=��	֠@t�����K蔄V���D�*n���d���J
ZI���恣h��G�:͋�8�/c�<-��C	��rr7]�ܙl2h�&��Ab�#�%���j��Gh���5�	�Q�ym_g���5I#+�j ][�����*l�����/�1��~��7�{�����7\�7��F�^���y肯׊��Z����iIwl
8[G
J���kWv��4Hܢ�{�r8rZ���Q�ݧ�I������Ks&=��X^�\�җW������P���k|a�|�W�PiҸ�u��K�9�>I!؎�tx_F� q*���R�|���O�[��I?���c����\h^=`Rk
��\�!��z��rF�^[�t��)�����[W�5��O���3�k�\�ԛ�JN4��,59Dxl�$��C�M��י�x�T{���3��7��Gh��h����̑ǭ�Y��Lh�T��y�m��o�p��}��B���ꧧ�a���M�_J?��Й,o�9M�a��?�&r�>j�����gItʄ[M���vπ�=��[���Z�p���k�xd�IU�8������<	[�]�ꃆt�5�J�)>D�Wl�d~�L�[5�63/�i��0o�����X���K�.vd��˘D4� ^(f_\��=zc��b篤����d�
1e��7��-0/~7>=����&I����~gEP�_Z�ͷBd�PQ\_�74��He"��ݍy��
9󓱁a_��cP�1�ϖa�f�ůɖ��}���VsP����u�v�%6f+!�C�-9�PDd��zhy��!=QF�nT�����ˣ�:���S��~�7�gvl�$)c��r4~���S�Lݲ�paR�ޫ0u �Jq,/h�m��:�LS�=�^����9�Wy7:q:����t=O=�����Z=J���=��u!�A�:ժ��j}�~NV{�Jd�3-�|k8~n
�ڌV�Ȏ���SH<�z�?����[@���fC~�O���ԯ��������
����+��61;��|�G!KS�`p0Q�CV�nf��2J����e�WZk���>æ�<)=�����	*�d�?�_I����A�S�0�M���86K��T&�@&Y�ܜ�J�}��k�!�s�N|{d����v�i���+v�+*~
����}�W����g��P����W�av�!��j ~����w�I&�7e��3&	��1�2Yo�p���N߈*7�u��#s?�WN�����eϝ�*zdU�Ց"`�����Z��>2pL��T�Ez�_1��u�7���E	yLWD��@}l�k�:�|#q��?��'?;�7�a���@�
��W��7�=�o�G�	
����=l�}h��� �)qنPuV��8�����e�-&O�۴����~�"A���d��!��Bԫ/��R�g�j�ȁX~c�Est�pf�_9��%_C_��kKr��%[x�R�bCHD�-^������Gn�4λݲK�R�K����>�_]EQ �?�¯��x��MΑ-b�a�Q@�eZ���Y�nO�������d�,�l�3��"���C��dT����W�t9�k�GsR[v��J@�M����+-MV�`��x]]���0qC�(�J��PR3��BU�z�,��<E���TG9+�Ow��}��Ƿ�^c-�|��'���&�`o�{R��J^�������,*��e�}����28O`�"-ym�0򧇳u#[G��C�S���g��	�u���f�ڂ)��A�h��jGV�A|�� +A�2 ��`��W��+���}��Щ��E�әq`�o<|��h;�k�� ��KC�Ͽ��ӫ��5c�䯽�&�`o�{V�5�3�Ϧ|"��<^/* �gݺ̠J�Sj��I�VUu��r�~��1#
�Y�`�۩ĕ�H������PG"=��'�ӂ����<��t���@tñQK��7p����n@�Ԝ'������M������#:�����~�;o����/�.��`����?�?�8�+�{#��'|ɨ�m�h�V��
//_��a&ӶC�� ^[>�����pTyb\fI8_����Fb�����J�;�$�>{�\���U��� ��ͼg�0XDKu�����#6IG#Հ�h ��E{B��\��
+�_gQ�U����,��X�/����1���7�~���ވ�&��+�=��C��8b�\eo0j#nD����G1�	5͕��ʉ�,�o�	tn�.OгF� ˇ���y,<!eQb��F�ji��4�/�e�[�� ���F��XU�c�/	�
��}�a.��`��Nrd���\����M�ϖ�7#~������&��\w_�e�zm0\�שO'��K��9˙�x u�	��9�{���`D`/�n�JQ!7���Z��f��!,jݕ^�	T�E߀��B͂_�ޏ��R�wC�+%g��(x|�[�d:\eư�[� �����D~���Of|U|bo�����i;��_<U��K�8����	w?$��y�>�-��܄���t�Ϳ��� �+�7���^}3�yd�L�3�I�����
������y��7�<n��F/�=:�j��=`K{jD��Q)=k�ѱ��dmk6�=_'�0��	=�|/����q���X^c�� ���O藺���������+�L      �      x������ � �     