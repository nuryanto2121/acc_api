PGDMP                         x            tsm_db    11.4    11.4 }    �           0    0    ENCODING    ENCODING        SET client_encoding = 'UTF8';
                       false            �           0    0 
   STDSTRINGS 
   STDSTRINGS     (   SET standard_conforming_strings = 'on';
                       false            �           0    0 
   SEARCHPATH 
   SEARCHPATH     8   SELECT pg_catalog.set_config('search_path', '', false);
                       false            �           1262    35220    tsm_db    DATABASE     �   CREATE DATABASE tsm_db WITH TEMPLATE = template0 ENCODING = 'UTF8' LC_COLLATE = 'English_United States.1252' LC_CTYPE = 'English_United States.1252';
    DROP DATABASE tsm_db;
             postgres    false            �            1255    35648 G   _getlogin_auth(character varying, character varying, character varying)    FUNCTION     �  CREATE FUNCTION public._getlogin_auth(p_user_id character varying, p_password character varying, p_subportfolio_short_name character varying DEFAULT ''::character varying) RETURNS TABLE(user_id character varying, is_inactive character varying, user_name character varying, password character varying, user_group_short_descs character varying, user_group_descs character varying, subportfolio_short_name character varying, subportfolio_name character varying, portfolio_short_name character varying, portfolio_name character varying, default_language character varying, ss_group_id integer, portfolio_id integer, subportfolio_id integer)
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
       public       postgres    false            �            1255    35559 !   fss_menu_list_s(integer, integer)    FUNCTION     �  CREATE FUNCTION public.fss_menu_list_s(p_portfolio_id integer, p_group_id integer) RETURNS TABLE(ss_menu_id integer, title character varying, menu_url character varying, menu_type character varying, parent_menu_id integer, icon_class character varying, order_seq integer, level integer, ipath character varying, add_status boolean, edit_status boolean, delete_status boolean)
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
       public       postgres    false            �            1255    35650 =   fss_menu_sort_u(integer, integer, integer, character varying)    FUNCTION     �  CREATE FUNCTION public.fss_menu_sort_u(p_menu_id integer, p_parent_menu_id integer, p_order_seq integer, p_user_edit character varying) RETURNS character varying
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
       public       postgres    false            �            1255    35649 w   fss_option_db_i(character varying, character varying, character varying, integer, character varying, character varying)    FUNCTION     �  CREATE FUNCTION public.fss_option_db_i(p_option_url character varying, p_method_api character varying, p_sp character varying, p_line_no integer, p_user_input character varying, p_table_name character varying DEFAULT ''::character varying) RETURNS integer
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
       public       postgres    false            �            1255    35335 �  fss_user_log_i(timestamp without time zone, character varying, character varying, character varying, character varying, character varying, timestamp without time zone, timestamp without time zone, character varying, character varying, character varying, character varying, timestamp without time zone, timestamp without time zone, character varying, timestamp without time zone, character varying)    FUNCTION     �  CREATE FUNCTION public.fss_user_log_i(p_log_status timestamp without time zone, p_user_id character varying, p_user_group character varying, p_user_name character varying, p_email character varying, p_user_level character varying, p_expired_date timestamp without time zone, p_login_date timestamp without time zone, p_status_login character varying, p_is_inactive character varying, p_user_input character varying, p_user_edit character varying, p_time_input timestamp without time zone, p_time_edit timestamp without time zone, p_ip_address character varying, p_logout_date timestamp without time zone, p_token character varying) RETURNS character varying
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
       public       postgres    false            �            1255    35651 %   get_param_function(character varying)    FUNCTION     .  CREATE FUNCTION public.get_param_function(function_name character varying) RETURNS TABLE(routine_name information_schema.sql_identifier, parameter_name information_schema.sql_identifier, data_type information_schema.character_data, oridinal_position information_schema.cardinal_number)
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
       public       postgres    false            �            1259    35297    ss_portfolio    TABLE     "  CREATE TABLE public.ss_portfolio (
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
       public         postgres    false            �            1259    35295     cm_portfolio_cm_portfolio_id_seq    SEQUENCE     �   CREATE SEQUENCE public.cm_portfolio_cm_portfolio_id_seq
    AS integer
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;
 7   DROP SEQUENCE public.cm_portfolio_cm_portfolio_id_seq;
       public       postgres    false    205            �           0    0     cm_portfolio_cm_portfolio_id_seq    SEQUENCE OWNED BY     e   ALTER SEQUENCE public.cm_portfolio_cm_portfolio_id_seq OWNED BY public.ss_portfolio.ss_portfolio_id;
            public       postgres    false    204            �            1259    35455    ss_subportfolio    TABLE     C  CREATE TABLE public.ss_subportfolio (
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
       public         postgres    false            �            1259    35453 *   cm_subportfolio_new_cm_subportfolio_id_seq    SEQUENCE     �   CREATE SEQUENCE public.cm_subportfolio_new_cm_subportfolio_id_seq
    AS integer
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;
 A   DROP SEQUENCE public.cm_subportfolio_new_cm_subportfolio_id_seq;
       public       postgres    false    211            �           0    0 *   cm_subportfolio_new_cm_subportfolio_id_seq    SEQUENCE OWNED BY     u   ALTER SEQUENCE public.cm_subportfolio_new_cm_subportfolio_id_seq OWNED BY public.ss_subportfolio.ss_subportfolio_id;
            public       postgres    false    210            �            1259    35629    ss_dashboard_group    TABLE     �  CREATE TABLE public.ss_dashboard_group (
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
       public         postgres    false            �            1259    35627 ,   ss_dashboard_group_ss_dashboard_group_id_seq    SEQUENCE     �   CREATE SEQUENCE public.ss_dashboard_group_ss_dashboard_group_id_seq
    AS integer
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;
 C   DROP SEQUENCE public.ss_dashboard_group_ss_dashboard_group_id_seq;
       public       postgres    false    226            �           0    0 ,   ss_dashboard_group_ss_dashboard_group_id_seq    SEQUENCE OWNED BY     }   ALTER SEQUENCE public.ss_dashboard_group_ss_dashboard_group_id_seq OWNED BY public.ss_dashboard_group.ss_dashboard_group_id;
            public       postgres    false    225            �            1259    35545    ss_group    TABLE     �  CREATE TABLE public.ss_group (
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
       public         postgres    false            �            1259    35543    ss_group_new_ss_group_id_seq    SEQUENCE     �   CREATE SEQUENCE public.ss_group_new_ss_group_id_seq
    AS integer
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;
 3   DROP SEQUENCE public.ss_group_new_ss_group_id_seq;
       public       postgres    false    215            �           0    0    ss_group_new_ss_group_id_seq    SEQUENCE OWNED BY     Y   ALTER SEQUENCE public.ss_group_new_ss_group_id_seq OWNED BY public.ss_group.ss_group_id;
            public       postgres    false    214            �            1259    35469    ss_menu    TABLE     B  CREATE TABLE public.ss_menu (
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
       public         postgres    false            �            1259    35618    ss_menu_dashboard    TABLE     �  CREATE TABLE public.ss_menu_dashboard (
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
       public         postgres    false            �            1259    35616 ,   ss_menu_dashboard_ss_master_dashboard_id_seq    SEQUENCE     �   CREATE SEQUENCE public.ss_menu_dashboard_ss_master_dashboard_id_seq
    AS integer
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;
 C   DROP SEQUENCE public.ss_menu_dashboard_ss_master_dashboard_id_seq;
       public       postgres    false    224            �           0    0 ,   ss_menu_dashboard_ss_master_dashboard_id_seq    SEQUENCE OWNED BY     }   ALTER SEQUENCE public.ss_menu_dashboard_ss_master_dashboard_id_seq OWNED BY public.ss_menu_dashboard.ss_master_dashboard_id;
            public       postgres    false    223            �            1259    35584    ss_menu_group    TABLE     �  CREATE TABLE public.ss_menu_group (
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
       public         postgres    false            �            1259    35582 &   ss_menu_group_new_ss_menu_group_id_seq    SEQUENCE     �   CREATE SEQUENCE public.ss_menu_group_new_ss_menu_group_id_seq
    AS integer
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;
 =   DROP SEQUENCE public.ss_menu_group_new_ss_menu_group_id_seq;
       public       postgres    false    219            �           0    0 &   ss_menu_group_new_ss_menu_group_id_seq    SEQUENCE OWNED BY     m   ALTER SEQUENCE public.ss_menu_group_new_ss_menu_group_id_seq OWNED BY public.ss_menu_group.ss_menu_group_id;
            public       postgres    false    218            �            1259    35467    ss_menu_ss_menu_id_seq    SEQUENCE     �   CREATE SEQUENCE public.ss_menu_ss_menu_id_seq
    AS integer
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;
 -   DROP SEQUENCE public.ss_menu_ss_menu_id_seq;
       public       postgres    false    213            �           0    0    ss_menu_ss_menu_id_seq    SEQUENCE OWNED BY     Q   ALTER SEQUENCE public.ss_menu_ss_menu_id_seq OWNED BY public.ss_menu.ss_menu_id;
            public       postgres    false    212            �            1259    35424 	   ss_module    TABLE     l  CREATE TABLE public.ss_module (
    ss_module_id integer NOT NULL,
    descs character varying(150) NOT NULL,
    short_descs character varying(60) NOT NULL,
    user_input character varying(10) NOT NULL,
    user_edit character varying(10) NOT NULL,
    time_input timestamp(0) without time zone NOT NULL,
    time_edit timestamp(0) without time zone NOT NULL
);
    DROP TABLE public.ss_module;
       public         postgres    false            �            1259    35422    ss_module_ss_module_id_seq    SEQUENCE     �   CREATE SEQUENCE public.ss_module_ss_module_id_seq
    AS integer
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;
 1   DROP SEQUENCE public.ss_module_ss_module_id_seq;
       public       postgres    false    209            �           0    0    ss_module_ss_module_id_seq    SEQUENCE OWNED BY     Y   ALTER SEQUENCE public.ss_module_ss_module_id_seq OWNED BY public.ss_module.ss_module_id;
            public       postgres    false    208            �            1259    35245    ss_option_db    TABLE     �  CREATE TABLE public.ss_option_db (
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
       public         postgres    false            �            1259    35243     ss_option_db_ss_option_db_id_seq    SEQUENCE     �   CREATE SEQUENCE public.ss_option_db_ss_option_db_id_seq
    AS integer
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;
 7   DROP SEQUENCE public.ss_option_db_ss_option_db_id_seq;
       public       postgres    false    199            �           0    0     ss_option_db_ss_option_db_id_seq    SEQUENCE OWNED BY     e   ALTER SEQUENCE public.ss_option_db_ss_option_db_id_seq OWNED BY public.ss_option_db.ss_option_db_id;
            public       postgres    false    198            �            1259    35257    ss_option_function    TABLE     �  CREATE TABLE public.ss_option_function (
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
       public         postgres    false            �            1259    35255 ,   ss_option_function_ss_option_function_id_seq    SEQUENCE     �   CREATE SEQUENCE public.ss_option_function_ss_option_function_id_seq
    AS integer
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;
 C   DROP SEQUENCE public.ss_option_function_ss_option_function_id_seq;
       public       postgres    false    201            �           0    0 ,   ss_option_function_ss_option_function_id_seq    SEQUENCE OWNED BY     }   ALTER SEQUENCE public.ss_option_function_ss_option_function_id_seq OWNED BY public.ss_option_function.ss_option_function_id;
            public       postgres    false    200            �            1259    35270    ss_option_lookup    TABLE     5  CREATE TABLE public.ss_option_lookup (
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
       public         postgres    false            �            1259    35268 (   ss_option_lookup_ss_option_lookup_id_seq    SEQUENCE     �   CREATE SEQUENCE public.ss_option_lookup_ss_option_lookup_id_seq
    AS integer
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;
 ?   DROP SEQUENCE public.ss_option_lookup_ss_option_lookup_id_seq;
       public       postgres    false    203            �           0    0 (   ss_option_lookup_ss_option_lookup_id_seq    SEQUENCE OWNED BY     u   ALTER SEQUENCE public.ss_option_lookup_ss_option_lookup_id_seq OWNED BY public.ss_option_lookup.ss_option_lookup_id;
            public       postgres    false    202            �            1259    35223    ss_user    TABLE     z  CREATE TABLE public.ss_user (
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
       public         postgres    false            �            1259    35574    ss_user_favorite    TABLE     �  CREATE TABLE public.ss_user_favorite (
    ss_user_favorite_id integer NOT NULL,
    ss_portfolio_id integer NOT NULL,
    user_id character varying(20) NOT NULL,
    ss_menu_id_id integer NOT NULL,
    user_input character varying(20) NOT NULL,
    user_edit character varying(20) NOT NULL,
    time_input timestamp(0) without time zone DEFAULT now(),
    time_edit timestamp(0) without time zone DEFAULT now()
);
 $   DROP TABLE public.ss_user_favorite;
       public         postgres    false            �            1259    35572 ,   ss_user_favorite_new_ss_user_favorite_id_seq    SEQUENCE     �   CREATE SEQUENCE public.ss_user_favorite_new_ss_user_favorite_id_seq
    AS integer
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;
 C   DROP SEQUENCE public.ss_user_favorite_new_ss_user_favorite_id_seq;
       public       postgres    false    217            �           0    0 ,   ss_user_favorite_new_ss_user_favorite_id_seq    SEQUENCE OWNED BY     y   ALTER SEQUENCE public.ss_user_favorite_new_ss_user_favorite_id_seq OWNED BY public.ss_user_favorite.ss_user_favorite_id;
            public       postgres    false    216            �            1259    35606    ss_user_log    TABLE     A  CREATE TABLE public.ss_user_log (
    ss_user_log_id integer NOT NULL,
    user_id character varying(20) NOT NULL,
    ip_address character varying(60),
    login_date timestamp(0) without time zone,
    logout_date timestamp(0) without time zone,
    token character varying(1000) NOT NULL,
    is_fraud boolean DEFAULT true NOT NULL,
    captcha character varying(60),
    user_input character varying(10) NOT NULL,
    user_edit character varying(10) NOT NULL,
    time_input timestamp(0) without time zone NOT NULL,
    time_edit timestamp(0) without time zone NOT NULL
);
    DROP TABLE public.ss_user_log;
       public         postgres    false            �            1259    35604    ss_user_log_ss_user_log_id_seq    SEQUENCE     �   CREATE SEQUENCE public.ss_user_log_ss_user_log_id_seq
    AS integer
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;
 5   DROP SEQUENCE public.ss_user_log_ss_user_log_id_seq;
       public       postgres    false    222            �           0    0    ss_user_log_ss_user_log_id_seq    SEQUENCE OWNED BY     a   ALTER SEQUENCE public.ss_user_log_ss_user_log_id_seq OWNED BY public.ss_user_log.ss_user_log_id;
            public       postgres    false    221            �            1259    35313    ss_user_session    TABLE     �  CREATE TABLE public.ss_user_session (
    user_session_id integer NOT NULL,
    user_id character varying(10) NOT NULL,
    token character varying(1000) NOT NULL,
    last_login timestamp(0) without time zone,
    expire_on timestamp(0) without time zone,
    ip_address character varying(20),
    user_input character varying(10) NOT NULL,
    user_edit character varying(10) NOT NULL,
    time_input timestamp(0) without time zone NOT NULL,
    time_edit timestamp(0) without time zone NOT NULL
);
 #   DROP TABLE public.ss_user_session;
       public         postgres    false            �            1259    35311 #   ss_user_session_user_session_id_seq    SEQUENCE     �   CREATE SEQUENCE public.ss_user_session_user_session_id_seq
    AS integer
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;
 :   DROP SEQUENCE public.ss_user_session_user_session_id_seq;
       public       postgres    false    207            �           0    0 #   ss_user_session_user_session_id_seq    SEQUENCE OWNED BY     k   ALTER SEQUENCE public.ss_user_session_user_session_id_seq OWNED BY public.ss_user_session.user_session_id;
            public       postgres    false    206            �            1259    35221    ss_user_ss_user_id_seq    SEQUENCE     �   CREATE SEQUENCE public.ss_user_ss_user_id_seq
    AS integer
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;
 -   DROP SEQUENCE public.ss_user_ss_user_id_seq;
       public       postgres    false    197            �           0    0    ss_user_ss_user_id_seq    SEQUENCE OWNED BY     Q   ALTER SEQUENCE public.ss_user_ss_user_id_seq OWNED BY public.ss_user.ss_user_id;
            public       postgres    false    196            �            1259    35640    ss_user_subportfolio    TABLE     �  CREATE TABLE public.ss_user_subportfolio (
    ss_user_subportfolio_id integer NOT NULL,
    user_id character varying(20) NOT NULL,
    subportfolio_id integer NOT NULL,
    user_input character varying(10) NOT NULL,
    user_edit character varying(10) NOT NULL,
    time_input timestamp(0) without time zone DEFAULT now() NOT NULL,
    time_edit timestamp(0) without time zone DEFAULT now() NOT NULL
);
 (   DROP TABLE public.ss_user_subportfolio;
       public         postgres    false            �            1259    35638 0   ss_user_subportfolio_ss_user_subportfolio_id_seq    SEQUENCE     �   CREATE SEQUENCE public.ss_user_subportfolio_ss_user_subportfolio_id_seq
    AS integer
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;
 G   DROP SEQUENCE public.ss_user_subportfolio_ss_user_subportfolio_id_seq;
       public       postgres    false    228            �           0    0 0   ss_user_subportfolio_ss_user_subportfolio_id_seq    SEQUENCE OWNED BY     �   ALTER SEQUENCE public.ss_user_subportfolio_ss_user_subportfolio_id_seq OWNED BY public.ss_user_subportfolio.ss_user_subportfolio_id;
            public       postgres    false    227            �            1259    35599    vget_menu_group    VIEW     �  CREATE VIEW public.vget_menu_group AS
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
       public       postgres    false    213    219    219    219    219    219    213    213    213    213    213    213    219                       2604    35632 (   ss_dashboard_group ss_dashboard_group_id    DEFAULT     �   ALTER TABLE ONLY public.ss_dashboard_group ALTER COLUMN ss_dashboard_group_id SET DEFAULT nextval('public.ss_dashboard_group_ss_dashboard_group_id_seq'::regclass);
 W   ALTER TABLE public.ss_dashboard_group ALTER COLUMN ss_dashboard_group_id DROP DEFAULT;
       public       postgres    false    226    225    226            �
           2604    35548    ss_group ss_group_id    DEFAULT     �   ALTER TABLE ONLY public.ss_group ALTER COLUMN ss_group_id SET DEFAULT nextval('public.ss_group_new_ss_group_id_seq'::regclass);
 C   ALTER TABLE public.ss_group ALTER COLUMN ss_group_id DROP DEFAULT;
       public       postgres    false    214    215    215            �
           2604    35472    ss_menu ss_menu_id    DEFAULT     x   ALTER TABLE ONLY public.ss_menu ALTER COLUMN ss_menu_id SET DEFAULT nextval('public.ss_menu_ss_menu_id_seq'::regclass);
 A   ALTER TABLE public.ss_menu ALTER COLUMN ss_menu_id DROP DEFAULT;
       public       postgres    false    212    213    213            	           2604    35621 (   ss_menu_dashboard ss_master_dashboard_id    DEFAULT     �   ALTER TABLE ONLY public.ss_menu_dashboard ALTER COLUMN ss_master_dashboard_id SET DEFAULT nextval('public.ss_menu_dashboard_ss_master_dashboard_id_seq'::regclass);
 W   ALTER TABLE public.ss_menu_dashboard ALTER COLUMN ss_master_dashboard_id DROP DEFAULT;
       public       postgres    false    223    224    224            �
           2604    35587    ss_menu_group ss_menu_group_id    DEFAULT     �   ALTER TABLE ONLY public.ss_menu_group ALTER COLUMN ss_menu_group_id SET DEFAULT nextval('public.ss_menu_group_new_ss_menu_group_id_seq'::regclass);
 M   ALTER TABLE public.ss_menu_group ALTER COLUMN ss_menu_group_id DROP DEFAULT;
       public       postgres    false    218    219    219            �
           2604    35427    ss_module ss_module_id    DEFAULT     �   ALTER TABLE ONLY public.ss_module ALTER COLUMN ss_module_id SET DEFAULT nextval('public.ss_module_ss_module_id_seq'::regclass);
 E   ALTER TABLE public.ss_module ALTER COLUMN ss_module_id DROP DEFAULT;
       public       postgres    false    209    208    209            �
           2604    35248    ss_option_db ss_option_db_id    DEFAULT     �   ALTER TABLE ONLY public.ss_option_db ALTER COLUMN ss_option_db_id SET DEFAULT nextval('public.ss_option_db_ss_option_db_id_seq'::regclass);
 K   ALTER TABLE public.ss_option_db ALTER COLUMN ss_option_db_id DROP DEFAULT;
       public       postgres    false    198    199    199            �
           2604    35260 (   ss_option_function ss_option_function_id    DEFAULT     �   ALTER TABLE ONLY public.ss_option_function ALTER COLUMN ss_option_function_id SET DEFAULT nextval('public.ss_option_function_ss_option_function_id_seq'::regclass);
 W   ALTER TABLE public.ss_option_function ALTER COLUMN ss_option_function_id DROP DEFAULT;
       public       postgres    false    200    201    201            �
           2604    35273 $   ss_option_lookup ss_option_lookup_id    DEFAULT     �   ALTER TABLE ONLY public.ss_option_lookup ALTER COLUMN ss_option_lookup_id SET DEFAULT nextval('public.ss_option_lookup_ss_option_lookup_id_seq'::regclass);
 S   ALTER TABLE public.ss_option_lookup ALTER COLUMN ss_option_lookup_id DROP DEFAULT;
       public       postgres    false    203    202    203            �
           2604    35300    ss_portfolio ss_portfolio_id    DEFAULT     �   ALTER TABLE ONLY public.ss_portfolio ALTER COLUMN ss_portfolio_id SET DEFAULT nextval('public.cm_portfolio_cm_portfolio_id_seq'::regclass);
 K   ALTER TABLE public.ss_portfolio ALTER COLUMN ss_portfolio_id DROP DEFAULT;
       public       postgres    false    205    204    205            �
           2604    35458 "   ss_subportfolio ss_subportfolio_id    DEFAULT     �   ALTER TABLE ONLY public.ss_subportfolio ALTER COLUMN ss_subportfolio_id SET DEFAULT nextval('public.cm_subportfolio_new_cm_subportfolio_id_seq'::regclass);
 Q   ALTER TABLE public.ss_subportfolio ALTER COLUMN ss_subportfolio_id DROP DEFAULT;
       public       postgres    false    211    210    211            �
           2604    35226    ss_user ss_user_id    DEFAULT     x   ALTER TABLE ONLY public.ss_user ALTER COLUMN ss_user_id SET DEFAULT nextval('public.ss_user_ss_user_id_seq'::regclass);
 A   ALTER TABLE public.ss_user ALTER COLUMN ss_user_id DROP DEFAULT;
       public       postgres    false    196    197    197            �
           2604    35577 $   ss_user_favorite ss_user_favorite_id    DEFAULT     �   ALTER TABLE ONLY public.ss_user_favorite ALTER COLUMN ss_user_favorite_id SET DEFAULT nextval('public.ss_user_favorite_new_ss_user_favorite_id_seq'::regclass);
 S   ALTER TABLE public.ss_user_favorite ALTER COLUMN ss_user_favorite_id DROP DEFAULT;
       public       postgres    false    216    217    217                       2604    35609    ss_user_log ss_user_log_id    DEFAULT     �   ALTER TABLE ONLY public.ss_user_log ALTER COLUMN ss_user_log_id SET DEFAULT nextval('public.ss_user_log_ss_user_log_id_seq'::regclass);
 I   ALTER TABLE public.ss_user_log ALTER COLUMN ss_user_log_id DROP DEFAULT;
       public       postgres    false    221    222    222            �
           2604    35316    ss_user_session user_session_id    DEFAULT     �   ALTER TABLE ONLY public.ss_user_session ALTER COLUMN user_session_id SET DEFAULT nextval('public.ss_user_session_user_session_id_seq'::regclass);
 N   ALTER TABLE public.ss_user_session ALTER COLUMN user_session_id DROP DEFAULT;
       public       postgres    false    206    207    207                       2604    35643 ,   ss_user_subportfolio ss_user_subportfolio_id    DEFAULT     �   ALTER TABLE ONLY public.ss_user_subportfolio ALTER COLUMN ss_user_subportfolio_id SET DEFAULT nextval('public.ss_user_subportfolio_ss_user_subportfolio_id_seq'::regclass);
 [   ALTER TABLE public.ss_user_subportfolio ALTER COLUMN ss_user_subportfolio_id DROP DEFAULT;
       public       postgres    false    228    227    228            �          0    35629    ss_dashboard_group 
   TABLE DATA               �   COPY public.ss_dashboard_group (ss_dashboard_group_id, ss_portfolio_id, ss_group_id, ss_master_dashboard_id, user_input, user_edit, time_input, time_edit) FROM stdin;
    public       postgres    false    226   l�       �          0    35545    ss_group 
   TABLE DATA               �   COPY public.ss_group (ss_group_id, ss_portfolio_id, descs, short_descs, user_type, user_input, user_edit, time_input, time_edit) FROM stdin;
    public       postgres    false    215   ��       �          0    35469    ss_menu 
   TABLE DATA               �   COPY public.ss_menu (ss_menu_id, title, menu_url, menu_type, parent_menu_id, icon_class, order_seq, ss_module_id, user_input, user_edit, time_input, time_edit, level_no) FROM stdin;
    public       postgres    false    213   ��       �          0    35618    ss_menu_dashboard 
   TABLE DATA               �   COPY public.ss_menu_dashboard (ss_master_dashboard_id, menu_url, title, short_title, order_seq, user_input, user_edit, time_input, time_edit) FROM stdin;
    public       postgres    false    224   
�       �          0    35584    ss_menu_group 
   TABLE DATA               �   COPY public.ss_menu_group (ss_menu_group_id, ss_portfolio_id, ss_menu_id, ss_group_id, add_status, edit_status, delete_status, view_status, post_status, user_input, user_edit, time_input, time_edit) FROM stdin;
    public       postgres    false    219   '�       �          0    35424 	   ss_module 
   TABLE DATA               s   COPY public.ss_module (ss_module_id, descs, short_descs, user_input, user_edit, time_input, time_edit) FROM stdin;
    public       postgres    false    209   ��       �          0    35245    ss_option_db 
   TABLE DATA               �   COPY public.ss_option_db (ss_option_db_id, option_url, method_api, sp, line_no, table_name, user_input, user_edit, time_input, time_edit) FROM stdin;
    public       postgres    false    199   ��       �          0    35257    ss_option_function 
   TABLE DATA               �   COPY public.ss_option_function (ss_option_function_id, option_function_cd, module_cd, sp_name, sp_param, user_input, user_edit, time_input, time_edit) FROM stdin;
    public       postgres    false    201   ��       �          0    35270    ss_option_lookup 
   TABLE DATA               (  COPY public.ss_option_lookup (ss_option_lookup_id, option_lookup_cd, column_db, view_name, source_field, source_where, display_lookup, is_lookup_list, is_asyn, string_query, user_input, user_edit, time_input, time_edit, master_url, lookup_db_descs, lookup_db_parameter, lookup_table) FROM stdin;
    public       postgres    false    203   �       �          0    35297    ss_portfolio 
   TABLE DATA               �   COPY public.ss_portfolio (ss_portfolio_id, name, short_name, reference_no, address, city, post_cd, phone_no, fax_no, website, rounding_factor, remarks, picture_file_name, reference_file_name, user_input, user_edit, time_input, time_edit) FROM stdin;
    public       postgres    false    205   *�       �          0    35455    ss_subportfolio 
   TABLE DATA               �  COPY public.ss_subportfolio (ss_subportfolio_id, ss_portfolio_id, name, short_name, subportfolio_start, internal_contact_id, reference_no, address, city, post_cd, phone_no, fax_no, tax_address, tax_city, tax_post_cd, tax_registration_no, tax_registration_date, tax_reference_no, standard_tax_running_cd, common_tax_running_cd, ar_withholding_tax_running_cd, ap_withholding_deduction_running_cd, ap_vat_deduction_running_cd, default_vat_charges_assignment, hold_withholding, hold_vat, remarks, picture_file_name, reference_file_name, website, email, url_picture_map, map_file_name, ref_map_file_name, coordinate, latitude, longitude, user_input, user_edit, time_input, time_edit) FROM stdin;
    public       postgres    false    211   ��       �          0    35223    ss_user 
   TABLE DATA                 COPY public.ss_user (ss_user_id, user_id, ss_group_id, user_name, password, email, user_level, expired_date, is_inactive, job_title, hand_phone, last_change_password, default_language, user_input, user_edit, portfolio_id, subportfolio_id, time_input, time_edit) FROM stdin;
    public       postgres    false    197   ��       �          0    35574    ss_user_favorite 
   TABLE DATA               �   COPY public.ss_user_favorite (ss_user_favorite_id, ss_portfolio_id, user_id, ss_menu_id_id, user_input, user_edit, time_input, time_edit) FROM stdin;
    public       postgres    false    217   W�       �          0    35606    ss_user_log 
   TABLE DATA               �   COPY public.ss_user_log (ss_user_log_id, user_id, ip_address, login_date, logout_date, token, is_fraud, captcha, user_input, user_edit, time_input, time_edit) FROM stdin;
    public       postgres    false    222   t�       �          0    35313    ss_user_session 
   TABLE DATA               �   COPY public.ss_user_session (user_session_id, user_id, token, last_login, expire_on, ip_address, user_input, user_edit, time_input, time_edit) FROM stdin;
    public       postgres    false    207   ��       �          0    35640    ss_user_subportfolio 
   TABLE DATA               �   COPY public.ss_user_subportfolio (ss_user_subportfolio_id, user_id, subportfolio_id, user_input, user_edit, time_input, time_edit) FROM stdin;
    public       postgres    false    228   Z      �           0    0     cm_portfolio_cm_portfolio_id_seq    SEQUENCE SET     N   SELECT pg_catalog.setval('public.cm_portfolio_cm_portfolio_id_seq', 2, true);
            public       postgres    false    204            �           0    0 *   cm_subportfolio_new_cm_subportfolio_id_seq    SEQUENCE SET     X   SELECT pg_catalog.setval('public.cm_subportfolio_new_cm_subportfolio_id_seq', 1, true);
            public       postgres    false    210            �           0    0 ,   ss_dashboard_group_ss_dashboard_group_id_seq    SEQUENCE SET     [   SELECT pg_catalog.setval('public.ss_dashboard_group_ss_dashboard_group_id_seq', 1, false);
            public       postgres    false    225            �           0    0    ss_group_new_ss_group_id_seq    SEQUENCE SET     K   SELECT pg_catalog.setval('public.ss_group_new_ss_group_id_seq', 1, false);
            public       postgres    false    214            �           0    0 ,   ss_menu_dashboard_ss_master_dashboard_id_seq    SEQUENCE SET     [   SELECT pg_catalog.setval('public.ss_menu_dashboard_ss_master_dashboard_id_seq', 1, false);
            public       postgres    false    223            �           0    0 &   ss_menu_group_new_ss_menu_group_id_seq    SEQUENCE SET     U   SELECT pg_catalog.setval('public.ss_menu_group_new_ss_menu_group_id_seq', 11, true);
            public       postgres    false    218            �           0    0    ss_menu_ss_menu_id_seq    SEQUENCE SET     E   SELECT pg_catalog.setval('public.ss_menu_ss_menu_id_seq', 23, true);
            public       postgres    false    212            �           0    0    ss_module_ss_module_id_seq    SEQUENCE SET     H   SELECT pg_catalog.setval('public.ss_module_ss_module_id_seq', 8, true);
            public       postgres    false    208            �           0    0     ss_option_db_ss_option_db_id_seq    SEQUENCE SET     N   SELECT pg_catalog.setval('public.ss_option_db_ss_option_db_id_seq', 1, true);
            public       postgres    false    198            �           0    0 ,   ss_option_function_ss_option_function_id_seq    SEQUENCE SET     [   SELECT pg_catalog.setval('public.ss_option_function_ss_option_function_id_seq', 1, false);
            public       postgres    false    200            �           0    0 (   ss_option_lookup_ss_option_lookup_id_seq    SEQUENCE SET     W   SELECT pg_catalog.setval('public.ss_option_lookup_ss_option_lookup_id_seq', 1, false);
            public       postgres    false    202            �           0    0 ,   ss_user_favorite_new_ss_user_favorite_id_seq    SEQUENCE SET     [   SELECT pg_catalog.setval('public.ss_user_favorite_new_ss_user_favorite_id_seq', 1, false);
            public       postgres    false    216            �           0    0    ss_user_log_ss_user_log_id_seq    SEQUENCE SET     N   SELECT pg_catalog.setval('public.ss_user_log_ss_user_log_id_seq', 109, true);
            public       postgres    false    221            �           0    0 #   ss_user_session_user_session_id_seq    SEQUENCE SET     R   SELECT pg_catalog.setval('public.ss_user_session_user_session_id_seq', 53, true);
            public       postgres    false    206            �           0    0    ss_user_ss_user_id_seq    SEQUENCE SET     D   SELECT pg_catalog.setval('public.ss_user_ss_user_id_seq', 1, true);
            public       postgres    false    196            �           0    0 0   ss_user_subportfolio_ss_user_subportfolio_id_seq    SEQUENCE SET     _   SELECT pg_catalog.setval('public.ss_user_subportfolio_ss_user_subportfolio_id_seq', 1, false);
            public       postgres    false    227                       2606    35437    ss_portfolio cm_portfolio_pkey 
   CONSTRAINT     ^   ALTER TABLE ONLY public.ss_portfolio
    ADD CONSTRAINT cm_portfolio_pkey PRIMARY KEY (name);
 H   ALTER TABLE ONLY public.ss_portfolio DROP CONSTRAINT cm_portfolio_pkey;
       public         postgres    false    205            %           2606    35465 :   ss_subportfolio cm_subportfolio_cm_new_subportfolio_id_key 
   CONSTRAINT     �   ALTER TABLE ONLY public.ss_subportfolio
    ADD CONSTRAINT cm_subportfolio_cm_new_subportfolio_id_key PRIMARY KEY (ss_subportfolio_id);
 d   ALTER TABLE ONLY public.ss_subportfolio DROP CONSTRAINT cm_subportfolio_cm_new_subportfolio_id_key;
       public         postgres    false    211            3           2606    35636 *   ss_dashboard_group ss_dashboard_group_pkey 
   CONSTRAINT     �   ALTER TABLE ONLY public.ss_dashboard_group
    ADD CONSTRAINT ss_dashboard_group_pkey PRIMARY KEY (ss_portfolio_id, ss_group_id, ss_master_dashboard_id);
 T   ALTER TABLE ONLY public.ss_dashboard_group DROP CONSTRAINT ss_dashboard_group_pkey;
       public         postgres    false    226    226    226            )           2606    35552    ss_group ss_group_new_pkey 
   CONSTRAINT     a   ALTER TABLE ONLY public.ss_group
    ADD CONSTRAINT ss_group_new_pkey PRIMARY KEY (ss_group_id);
 D   ALTER TABLE ONLY public.ss_group DROP CONSTRAINT ss_group_new_pkey;
       public         postgres    false    215            1           2606    35625 *   ss_menu_dashboard ss_master_dashboard_pkey 
   CONSTRAINT     y   ALTER TABLE ONLY public.ss_menu_dashboard
    ADD CONSTRAINT ss_master_dashboard_pkey PRIMARY KEY (menu_url, order_seq);
 T   ALTER TABLE ONLY public.ss_menu_dashboard DROP CONSTRAINT ss_master_dashboard_pkey;
       public         postgres    false    224    224            -           2606    35596 $   ss_menu_group ss_menu_group_new_pkey 
   CONSTRAINT     w   ALTER TABLE ONLY public.ss_menu_group
    ADD CONSTRAINT ss_menu_group_new_pkey PRIMARY KEY (ss_menu_id, ss_group_id);
 N   ALTER TABLE ONLY public.ss_menu_group DROP CONSTRAINT ss_menu_group_new_pkey;
       public         postgres    false    219    219            '           2606    35477    ss_menu ss_menu_pkey 
   CONSTRAINT     Z   ALTER TABLE ONLY public.ss_menu
    ADD CONSTRAINT ss_menu_pkey PRIMARY KEY (ss_menu_id);
 >   ALTER TABLE ONLY public.ss_menu DROP CONSTRAINT ss_menu_pkey;
       public         postgres    false    213            #           2606    35429    ss_module ss_module_id_key 
   CONSTRAINT     b   ALTER TABLE ONLY public.ss_module
    ADD CONSTRAINT ss_module_id_key PRIMARY KEY (ss_module_id);
 D   ALTER TABLE ONLY public.ss_module DROP CONSTRAINT ss_module_id_key;
       public         postgres    false    209                       2606    35252    ss_option_db ss_option_db_pkey 
   CONSTRAINT     i   ALTER TABLE ONLY public.ss_option_db
    ADD CONSTRAINT ss_option_db_pkey PRIMARY KEY (ss_option_db_id);
 H   ALTER TABLE ONLY public.ss_option_db DROP CONSTRAINT ss_option_db_pkey;
       public         postgres    false    199                       2606    35267 <   ss_option_function ss_option_function_option_function_cd_key 
   CONSTRAINT     �   ALTER TABLE ONLY public.ss_option_function
    ADD CONSTRAINT ss_option_function_option_function_cd_key UNIQUE (option_function_cd);
 f   ALTER TABLE ONLY public.ss_option_function DROP CONSTRAINT ss_option_function_option_function_cd_key;
       public         postgres    false    201                       2606    35265 ?   ss_option_function ss_option_function_ss_option_function_id_key 
   CONSTRAINT     �   ALTER TABLE ONLY public.ss_option_function
    ADD CONSTRAINT ss_option_function_ss_option_function_id_key PRIMARY KEY (ss_option_function_id);
 i   ALTER TABLE ONLY public.ss_option_function DROP CONSTRAINT ss_option_function_ss_option_function_id_key;
       public         postgres    false    201                       2606    35278 9   ss_option_lookup ss_option_lookup_ss_option_lookup_id_key 
   CONSTRAINT     �   ALTER TABLE ONLY public.ss_option_lookup
    ADD CONSTRAINT ss_option_lookup_ss_option_lookup_id_key PRIMARY KEY (ss_option_lookup_id);
 c   ALTER TABLE ONLY public.ss_option_lookup DROP CONSTRAINT ss_option_lookup_ss_option_lookup_id_key;
       public         postgres    false    203            +           2606    35581 *   ss_user_favorite ss_user_favorite_new_pkey 
   CONSTRAINT     �   ALTER TABLE ONLY public.ss_user_favorite
    ADD CONSTRAINT ss_user_favorite_new_pkey PRIMARY KEY (ss_portfolio_id, user_id, ss_menu_id_id);
 T   ALTER TABLE ONLY public.ss_user_favorite DROP CONSTRAINT ss_user_favorite_new_pkey;
       public         postgres    false    217    217    217            /           2606    35615 *   ss_user_log ss_user_log_ss_user_log_id_key 
   CONSTRAINT     t   ALTER TABLE ONLY public.ss_user_log
    ADD CONSTRAINT ss_user_log_ss_user_log_id_key PRIMARY KEY (ss_user_log_id);
 T   ALTER TABLE ONLY public.ss_user_log DROP CONSTRAINT ss_user_log_ss_user_log_id_key;
       public         postgres    false    222            !           2606    35321 6   ss_user_session ss_user_session_ss_user_session_id_key 
   CONSTRAINT     �   ALTER TABLE ONLY public.ss_user_session
    ADD CONSTRAINT ss_user_session_ss_user_session_id_key PRIMARY KEY (user_session_id);
 `   ALTER TABLE ONLY public.ss_user_session DROP CONSTRAINT ss_user_session_ss_user_session_id_key;
       public         postgres    false    207                       2606    35230    ss_user ss_user_ss_user_id_key 
   CONSTRAINT     d   ALTER TABLE ONLY public.ss_user
    ADD CONSTRAINT ss_user_ss_user_id_key PRIMARY KEY (ss_user_id);
 H   ALTER TABLE ONLY public.ss_user DROP CONSTRAINT ss_user_ss_user_id_key;
       public         postgres    false    197            5           2606    35647 .   ss_user_subportfolio ss_user_subportfolio_pkey 
   CONSTRAINT     �   ALTER TABLE ONLY public.ss_user_subportfolio
    ADD CONSTRAINT ss_user_subportfolio_pkey PRIMARY KEY (user_id, subportfolio_id);
 X   ALTER TABLE ONLY public.ss_user_subportfolio DROP CONSTRAINT ss_user_subportfolio_pkey;
       public         postgres    false    228    228                       2606    35232    ss_user ss_user_user_id_key 
   CONSTRAINT     Y   ALTER TABLE ONLY public.ss_user
    ADD CONSTRAINT ss_user_user_id_key UNIQUE (user_id);
 E   ALTER TABLE ONLY public.ss_user DROP CONSTRAINT ss_user_user_id_key;
       public         postgres    false    197            �      x������ � �      �   >   x�3�4�v�.-H-RpL������t��FF�ƺ&
�V�V���ĸb���� ���      �   #  x���M��0���_a�����-�{h�l���R���8[��F�ʿ���mh���0�66���#�hw*���p�˧�*�	v���)g�d)�'	�U�G�ܼy��0�,� �dY�K�rk�Ib�x�x�q����A�q�a�D�� C���]��8m��j�����"
2T
��QoW�Ͱ�\2О�ړ���@����K�c�DotC_��1߁dT���a���Za�F��S���k^u�j<b��)�aF$��9��v�+�2�<��f��k�!C�h���.�e���X�SqmmP��X&�A&I�+]^[k�J娔���0If�lK����z�Ј���F��6L���(,��.��e>:��&)1�%�_���u�*hl�m��^[F0JY�ӥ7��M�����2�'$;��)��P�gu�\�|�+�0#B�'{<���z�)[t���!���fѴ���u1e�{Sl�<���/�����W��O�u/�#�y��si�Boܯ����5���/�h��^,}���L� �.
4\3��#!�/���      �      x������ � �      �   p   x��һ�0�O�A���X $�`�D���t��8��s9���߫���Qr5�)��H]PX1V�V�	V�V�VffXY�Z�b���W#}�x9�DD��<      �   �   x�}��N�0���S���6M��"��B���$.�PD�M%x{�	$`Q.9|�?�%���DS�)}��{�N'���� ���J4+�VR�Z���0�5�0�Q��
��Gq}��+m�ʰ560�i)������_,�Jf�FK�4�D���J����I.��6#Q�\����l\I՛V�j�-qE���.ǰ�-񲗱��c�3lm�ss�+��a���ܮn�\>�!���/      �   L   x�3���x�ԼR�� �Wδ���\ ?�8��$��Ӏ�3��/�J��*X�61�=... Og      �      x������ � �      �      x������ � �      �   �   x�m��
�0D��W���{Mc�]+�o���Mlb-h���"�p�3�A�����w|ܺs�o�e��fE@lr|�|eo����6�#�LK�
F�V��]0|�9�����5;t�%�	Ա���2O�E��f�b$���W/s��ѻ�V��ic�h�*B��W;o��tQ.?$ ���H	�3��`+� x f�A�      �   �   x�uO�
�@=�_���$S��R\�^/��"�)���כ��M^^H �ò�qX:pb��,|�&M�Ed�	 /�V� ��V�up�W޻}8Ug�hqYڻ��x2-��IZ=�4����#ɼ�`kT�p��6l�?W�/����5��O��Y'��E#�)�ш/oh ��7����M6/o�~�RNR6�z�6���5BM      �   s   x�3�,N�4�.-H-RpL����,�r����IIs�w	5��	q���,)�K)rH�M���K���t����p���%�����@��X��D���������W� �%2      �      x������ � �      �   ^  x���ǎ�H��]O�wa���zR�=�ZшVt�ӯ�{{13��P�
�ʌ�2�¾=��lo� 
���?�?)��� �H��>a��� ����~�x�}˿m$~Q&J��fY���p�7���_����ш�ШO���=����}��s������XD��K�T��m����]T�兡�;?)qn�,@c��"��UyT�U����6E�K!6�;j�9n��t�`f�y�ڴ��K���3V@ί��z{,�y��"���F[7XE$7(�*��6	�3ĚU+x��祍��e�f
�/��ѾG�z�b>�>�/h!�'H���t�����y^�59�OH��'t�EWl����ZB���9gT֞��9�4�Q��Tr� y�ȆN,����H7��Z�&-�۵a�(k�4����\Zu���]U��=�k�=8�z��Sj�ZWb.jD�?�+B	T"aNc��{Z?c>�> �[E�[`�+�K�����a��"��t�"��+�����h��k�	"�����z�.�'�_� ֘��)� ���S�i�KC���#'���B��fb��]X��uGY&-����1g
T��%m�KnKr�)Ѻʍ�Q�"��+d.�͠m�͛z�74ԠR�u�.�n醑��Yތ
��A�h��ώQ��j_����)�3њNoV��1h�\��̐O�D�C\P��u�gfN�l���y�.I����b(�qsi�IBv��,�Xc����B� ���8,rGOC-^������3�X�#t��]D��,mγ��Y�-����2X��|�|��V����\�J�A�꜇i�Hn��\�O�Rx�����+��D�A�����>1�%��|��������~9�f�x7i$��L�����#�Ŀ��8��τi$��L���5ӭ�fd[����^QRC�W5:��<�>H���p�v���*�F㦖Sy���J�?탤��j� ���,Ž�A�Nv��Ye��UH!"=3�ǽ���iJ^\�X]~m[��p��t�ي�xu��l�l�(W�e� o���5��$��g�{6���ڸfn�<��:�3F�1y1�f��'�D�������E*E��g����E�P6��a������A��[�Q�y�j���xeb?�Gx��(_��8�}�����]�XkC�Siq)Sݻ���:�>(�KP���#�E�xJ�ǲ�$�o=x�s�}P�o��eF���#"<�a��v�@��A�g�)��>�2��i뉸��FޜXc@��[�ev\�{ <\Mh�c$�ʌ�E��WX.^E(	1g�]�8%W+ם�k������V��N�����'����)lL��?���>{y��W��[£�`U}���û����A�_��ny�3���tO��C`������:�n����$�y%��\����C��	B�H���@[���K�]�2�9+.M���A�8 ?��mE���o)��_�D�k<�h`*
���O,Vv�EA�oD��ݭ��ۗ���j������^�=��1h�.�;Np��1aR1ܓ�IAiR�6�-�����\b�a�.�BV	���m���V�Ϧ%���:ٶM��*��u��ֈ:oe��ҌN�w�Qˍɞ��4��x�d��'��
��t��g18傮�I��5;��l�1Y��n!�L�w8T������q��1h�.�D�C\�+������q�R�(�
�o��{��rA���9���`����@ ^���@+Xr	����a�"�t��8<�kUt�Y'���J_v��W&�mXDi�R���3٧	л�/OW
�{k�V�q(�m�x�T]����n#�)t}w��"��y�*�=��1h�.�uB9����1�]��u��ł�kw&��ڎ&�W�_ct�Ix�U2����Z�3��L�Y	X~vj�İ���
U�T�A�"���V~*���)b}�1�,}��=�Nx��N)� �]�K�R��Μ�M�놥�n��U�ǿ��#��"����F"��~��۟����uI �HVV����� Ͱ����lʓ)��I�t�^]G��-2V4D�ߜl�΋Q?K(OR� �R�r�ؗk�\�\Jq�I[Tp*��q /�����I�ۋ���1�3�įq��yή���F���_�G��Zm�=�^������/탢~{�}���w4G��¼�v�����3D�a��p�sI)�r�|�V+I�6��kU9�;_o�$�؊��+�F���O�K��6m8���F
���"�c5�����Tu��!�*5�oO[H�J�-񦝸�3����A�7ܴJ�RqetZMq	����_1h�K���_]�vP��"@zܯ~�{p���/��S��[�e֚�|lI,�S��l�ڏc��@��Y�1�
��_�6�<�00K`��ˀA�pq����zW"�Ɵ~���]�*I����
�dZ����ꝫ�����3O_�.��T	޴�� ��|�21"��,�^��@{�e����?-_�`�m���?>��u���ߖ��۽�}(����{mU'�����o��d�><- ��r$^Ӆ@@��'�_�b�k(�9/�c_�K���;���Uq=�Ԟ`"����)چ=&֞]<���p���޻6��P�II^N��U%f_����y*���M՜���*?e��?y�h����)|�F?c>�^��/r����q�T���/!���J�H�I"&類��EC�K)3�������6pSN�^[I6�Dӊ=�fi��!�Fců�%��N�UES檦�`"0�Z�˹����>�3�1�\����8����E���Cvo900=�bȭl�$�X�p��W��^?b>�^�~��0�'����i�j�����S��fҮ�
��)	(��p�{Y�s��h;����w�|�RfEz����E@�>6�;PP��&���}�[�YW晬<c%��uX�$ڒ���d`4�tu5{�q���(�))�t�"V%�N���%�(��J�!�R}r�X��IC/����G���Er���7�U��G��)�Z %'�Mg�c��l�*kp�d�� )%g4���r惛j�CkX�>3�~�f����[��EC�ٛ�-l��EK³ W��Ok	Dg��9wٰ��O�8������&�a��;�P{�Ћ6�V����Z�m}������3�����/*/��H|"Ǽ\�8x<ӰA�eS2抺,C[=D��q��Z�@h6N!�dP@#E�rC}�%X��'\�p�dMP�����r�6�v�ytD��0L���=ٖ &��M؛H�X����܄ʗ?��L�{�#�Ɍ�`��jh�iw���o��x����c>�^��y�|��x{i��\"���s��ʨ��2+^�����ӒJ�=�UoM#t{�q�<��N00M�.�����5t�5�D7��o5�����4PS4U�T�Xu��ј��#�@�P��/d�S�	�n�Ѹ��7�`S���jҌpukl��v��*{mv&��c
���5�����oP?� ����L�x�1���K�[�mJ����'��������;q6�b1���;'�j.cǒ��=em���W����\���a��G9��.C�t6��� d�-�%�q�Ҫ멅�Se7s�m#4��l{C���I-+�l�ͽ4X�	X�Q�_����Ϙ����O;������+�hZGD�܋'ۯ���J͝�2�Z������x��ʕݡ!��8G�w��Nډ ���Z$�{d������X���T[ޭ>�"�g���*�P�����L3���M��4�(#b�9�<	>X���uh�zl#02�������??>>�;%.�      �      x���Y���֭}��W�{�n�.�s���軡�1�i~��z�"�U�ro���y,��k�1q�/�bҮ��Ee�ܹݭ���Hie_Z3�P�Q�r����Y��ѕ����:6��v��F&�~_�ιc�U�s�x��ps*㭾vBZ ��飰V�I��N�1���f�nR�J<p2Z���#�|XU�2���Z3 /��"�{�n��ί���q�/B���������?0�N}1�����Ϣ���M��пcȟ
!�@��d�\5�b�y�&x���^�se,w���ɽ#�Қ��0Q%�1�fȑ���Zp�DPߛ�|�R�b�k��	)p�<��M������};�x��a�����T.���aȨ���)e:H
`��R����c��OƝ�mf����n׋!��]��;����H����m�;F(�dF�{c���.٤qD�B"s����D*��k_S<`ȓo`�kj®����	ݮ�:����O�//����0�W���kZA˕h��.}�$)^&��]�-�ҪW]�>��b�-�Ҕ��z��,	("`X��
�Ⱦ�G���0�ͮ?����^��jA��|����|����T�j���r�c[���3�;h���0�x^��@�9��T��Ņf<� .!��^>�',��M0Vz��1�L�B$���)�¤�\UbR�[X����GC��	���s.i��c�����X���1tE�)��)�w$�%Z��J��F�@�w�>��۴\�}*|�+�#��-'�;=�dt�"�&b�Tg��Dz�CPc��\
p�%�b�^�wK�XXo���9s�(f�-�Zy�+��y��s��^�K�l��;�T4bh�|>��y�.�[<	�Q� Ʊ��HXs3����U�/�Lp��վ��?p�ʹ��j��
_��S�x@@��l�e�ڍ� ��d=KC)�q?Je�_�j>�j���p27����6��'���g��fC��Q�Ͳ�<�
�L�i��S�O-�J�v%���M���!F��"�A�D>1ޞ�����4>A^ZH���9�8��K�o
F�$rB�(�)��U���S����G�����_�?0��V�bԟ�`/�CG�d.}��O�u����oȫ��1��7�$~<u��K! *�;�����B��̰l��l�o� �Vi�r��a=<6��_��I10�-0+���'j�b�����>�5>�^%��A���;�#�%�����!��������{�Pa�7��R7������^���I�@�w3_���������Tx`�����Fodv�r���@�)��t�'�nj΍���nvJPO]\��[X���D9 �� �ъ� eTӤ�}��jύ�B�R�=��xmj1d�	��e3L�2���t�t����ɒq�F]P]�n%x.ހ�ꯝ)om���l5����A����-�/�����i���))O�濮T4��Wp�f���;�$���Ma,tJbv.V��V�6�b��Gs2T�6�_�G.Ӛ��O������Ԇ	�IM3q�(��o����
�qS&���]�	0��@���t����`D,�0�4��]������ǣ��g
��]ݺ�{�G&7�@���$�w�Z��N���~ ț��3�F�Կ���/}[�}�_���������pN�-@(ּ<f C�(�	�kj�(l��p�){Ox���I��)[j������z�̧{q��N0�H���M/O���E}#�e��l��uw��_�>뗼�� �d~I|�3g�����5F8��-9f&��N�pu>�*�3�w�gEj˧�WCl�������зE�����+�^ZA��ke"�4��pR��+��g2����t�n	#iT���qX.��;9�\Y�|��.�������c��&#��e:hA��'Ò�3:�\o�eᛞ0^gZc6��(����_�a���4�,Ce� i�5���3� �	�~p>��P� ��@[�I�2Zo�L���/����}����C��b/����᭺=�%��PLwu�SњQl���H=�驍�ӂB%R�(L$�m�j����5��m����\�������06�|�nb��N�4��#J���-S58B�6��Ї_wG�M������T��=�̼��_����gxvbx���0yd]��=�=�q�q��&�ʑ�����n(����o��O����SC���������-�!�SS�i� 1�K �q�,�۸&�� �=��������\�D|���ap��Y�@*��gJ�5׬���ԙ�F�=+ģ����ȪW؎�RE�x	:&:�J�^i�my�cX �ɑ�g�k�ˊ}�:A�kQ|<.Kw��&)[�)��Lx�(�M�B���kc9�C��m�n�'�o������qvK����DԚX�ۂ�v�MOC��^C��0m�=�:!29�0�θg�rʝ���z(�����r�m�����BF�$):�W��k?�� Y �i�N�٪	zR�MT#kF~��wiK�pGG����B��w�>�"��nw�\UN�)��V��IU��@ �{������Q�z7���7�~_�}�_RTb,'TK�� �z��`�^�t3I��E[�����h5C�
,O��p�9�zB�N�� �K�Pz�/��t^&����� U�fg_\��scv≨�:Sn? �9d}"̬�ɹ��K��3���=�uoj�
b��*d�����e��W���\G�.��}���? ���C�`���Y�}�_��d������<*AgQh��?07�X_��n���n=��S�_�Ě������8Q͜���K@��u���ؙ�	q�Qn>�Vǣ>��S����>���=�/`��.s�(��WsbrO�\*Z�Ta_�m������pŸ�{8��M�(*�"�	U�������<��/}[�}�Կ��_�AN���<Fy�Z!{� @os��j���U��T�l��K��R>;�)}=������<�)� ����A�3D^���}X��dgK	�-�ؒeBSO�r`R]�'j��˵�i��ְrn^��q֥k��}�ŧ�1��?tX��Z(�������y�ɾy�*���G��x���w۽����z+���!�W��Xn�4�V�'S�
�g;0������-��A,�'��y��dr�{ǽ6�|0��T����0��[8A�"e�㙦�Y�����,Mp�#x�[�*MZ�6��P�||D�IӺL�B3�������bWZ��).�ʵ�K{��ݩO�k��K����n����̊yѻ"|B�=��qQ������;8�ل]O���bw�S��]��$��}�8�FN��`0I������A�t'����}^��4/���Sڕ#�3���y�"S�78穅�`עk�2�v�����Vڡ�� ���rR��[i��y���/�z�~����d��x�fOD�+>?@�B Qh�&��:0�i:0% CStN�	��<ʁ`E���|L�2�r%�"r�'�ϫ�*q4M�1ԃ�9��C�1������r�.o�=77��NIE��R�,L��� D�O�M��i%�e#H�Ea�����7乃>��?^��ͩO�k��K�����׻� �
", �C܌ ��%��d))zcfrv�	U�T�\�2��9|�� @��J
�ty(IZK�/1S߱m�a��=�H����+E��V�5T�mu�x��S�&��S���g��릂�o�,d�x��g5����J=�|�A��;��[i���~Y��~�ԛ�} �o�f���%�A�2�j	���b�;�<����gtTk���Z�ި=f���{�Kl; �\Y�6���x�b�.��f�J�R�d��\���g�%�ט��}*rI+��Lz��9��=��{�F{>�#���Ph^^�t�a=�0OB���`5���*�i��F���ϩ�ٯ��.�b�y��
-�$�4����՗��9�4��*��M��[*}�!$A�?ә������z�o�܅;���^p��I>�s�;�:��6ʙ��S�r�����Z\��Q��kv�    ���@�5x���,N��ŏ�jnv��HD����|�.J]}`?�E��Ҿ?�$�د��˳��e�����1�)���
�R��і��WZ�M��Y(�s1���GH�=D�M�9����o$��w�%jE��iQ%5?|W!C���Q���oj��j�r��C���T��&�:��GÜ쥓�.��%e�β���z�f�@M�:'>��x�ҿ������5��?��K@�dȧ �A7����@�^\W&m�1ͨ5�K��(���d!����&��![$Mw���"���j���!Ov؆����}�"l�����3�{�݈&���ͧ-����2'�j�@��\�kkv�y��}��$$]H��n���m#P�L4��g�����o�a�o�|}w�w�K�ޤ_�!?S�](l��g���Atw(�IN�d���jhd>\��ޛp�P*�2�$<^y�IuC\|>3���"�05�.��������-���1e�iU��D�d.<e�����޺�8i�Ɋ.�-q������*���(�5�<<�g;ȋ��5g�J�~@��y��������@>��l��.6G`�2��n��̻���C.s�F���`�U�s�H"�{)2{��\��a6�MT=bJ� `ƃT.qX��Rk��V`W�E�����`vE���2	�+J�y�1e$,t��\h�6���-�f����D	5O�5^{�I��vi�VB���7�~g_�D��ԛ�} �ќv�I
�,uK��q��KÄ�6�QmQW7��[~Q���9(����/��8V�ѹVg�_tLc^�K
#�9��!�i�pA�U�kצ�1�W�6���������:�3j�Lv��`���A�t�>�q=C�U����ً�v��X�]����U��=���0��~r�zw�o�?��3�W���h?j���+J���G�Q�B@̢wl�/�sq${|�@Zq�m��	�ihM�h�1cc;�S�}~
�Ź,��,��pvP�U0�B��4
^���1���S<	� �-�}�B�}����ә�>�AZ��5c�	�TfhX��Ii)o���{�7�~g�t<���'��~nW�JW�[�)xv�]�@:�+�00\�k��7��I�� 9gU�W���AG�5�����
��:��>{���0�>&�&X�ԥ%Q�;V���!(#���n��j�Wk]�ꥼ����㩃a�Jo���C�@�	�j�
w1#O�H�(-Љ�;�9��a?6���'�eO}���>��hN:���s���Bi��ψ�44Q���R�K��u�V؞�8�qlZmi17���d|�X�37�gr+�hۤN�;����+T.�ݨ�͛z�w���Q�;���u�w+7�T���nR��	G�Lt<�"�w��v��,9�]���4�-!B��N���٩�Hb�g4�ȕ���ԕ)��w`lN@��CK,&��.$��JD݂�8� ͼ�I�h�2H?���p��_�"���rR>8:0�v�E�⚽r�0�:u>a×�|��Ҽw��[��"��2.��c���$�ȅ{�Nj�MȈ.�ӏĭת����ؾ����ط��'�����O����pu��j~3g�R2����S����f��]�����z�������/	A�j\4)��ph����Y��D����<8��L��$��Bw�({�'εpr�4lp�Q�2Y��OD�ka�����hlnt-!�d=�و��J��,�2p*�wU�o��N��������7ҟ���uE���M�g��I_7�+x��Ͷ��<���v�ލ��E�e��pr3��R���Az4q��փ�p.-sp�BJ��WMy�x��X��S��G`�Z#ŏ(�=�C���A_ݭ�a�Cß���vGn]��n��h����"w�#7.zؤs*������^	~w�w���z���>�OAx:fe�]���1'66#��vF���O���Ǵu�c�'�A����ɦ�B� �j]K�`�~21h=�ǔQ���R1q�� ��k���$UҼ"L2�m/H|��x(Y�R���1a�I���>m���,�	`�dd).�<���Ӽ���?0�ͩ��/��.��}��Mx:ϲ-mk�ǅy�Y��U�cuH�"*��v�a�R7W�aN�^���kZ�Ĳ�o��)?�d�����#�*n��kf���
D��_��٠�M[�V�1�4p]qr���Quϕ=��D�L��8��obu5�6ˍD��,R���\X�o�῕�@ߜ���©w�O�����6���ܐ
*?����m�p�֘��\7P�pb��0�Y�X����@�]M��cE�K��E���x~�_�$ĝup%H�V�;Mw�	$��.�.p�>�6ml8���C�b�:���Xp�N�!��W���E�	��+�k�r��J#~��
����`�p�]��@?3z����)��0|�P�<H��&�5wDlם��wT��\d�Ņ�P��c�]&n�
�V���\�[�߮U�RV>/i��J�$Ad�eۊ?�$�R׿��8z�x���TDQ�X8�z���=|�h����w>;8�S֢�ٳ���[i�UG�9�;��Sߥ?��3�{�����d/
�PjXz��)<,vR���c��+T����$ES]O���9"���Å�.��5�d[D�{�_tqŖ��<`��:���2:��y���՘{�K��tc�k 䂮�It�B�ˌl���B5w��Ħ烀K���s�^�Gߝ�d�v��'��}ft�zv���0ntj�՚�Ak�L��)\�74�C��jN����+ _[On�9;���뇇u��/D�6�աW�zV7�V���|e��F����jbf��4w���N#�`mz�I���f_;^$���޹���0ˉ��iPD���G�^����7�~g�t��'��}f�x�LN����qI�0��o|�29ZΣ0��j��{�M�L`0�K^9�cֺ��`�W%��>�"�[��[��9�t���z��	ѽn�ҡx�L CW����	��I@�L]�@���BtJ�LC"�Jܝ5Ǻh�OKݦ�3$랈�K�~����u��کoҟ���уv;��`�SPOj*E'��J,ծ�1,7Ҙ�yʮl��kO7�u�5���W֊ƨ6���M~�Q�y�$��Ц�žݳc�*�Y]O���
-�Pw�e��>�$��e���8�F�7��<��ݯ�!��~#O}�5�h�)諒?K���ک�g	��3�K�Y�vgؓ��I�d��쏄�p)1�v�}�QkQ�>?T��Y^XO�s�plE~&Ǌ�3������x3�C6���NĊ�"��i
�#5�V��c*�j��-�O�*խ�-'��V�$��;nZ�c%]	erzMqI���[i�S��y�oN��~1��Kb�gF���W�t�����M)�U<W�+�ǟ�C����$b�4�^�{��!�@�.�%b��Q��Y�{�.��WJ#�{�D�M~�`J�E���Ќ�²���mZ�=��.y�8�<s;a��iM�&%����V�C8�����6���u�	�b����J���لU�vQ�6V@C;�m�@WqwY�5�](�aJ��c!OD�쯯�b����D�_�Jx�gK�䢤#}�����)}&��2�Uz\mÞ��n��>O�Fem��ݮ�t��b�W�%iu���@'�tG!x]�x>�T�)\�i�s k'�����%|+��|
�b����V��>�ό�\*�n�8��yrE��E��ꙁ�Z4&����:b��5�����C�ja�&�+j��.Z6�`�ub.��6+~?#��Zj��:Z2W5��#43�t�����:K2���T��,.V��?�E����`9��a��q����r���&^	�4���;��~�So�/��}
�CO��[)��kH�l�O�`����Uͬ��`�����5obfEz�\a��ˀ���(�`I�k>\��W	�ǎ�6e������裉�%J�T`t�xw5�9��ܹs�Ԕs7�?P��&'WR��4�Zj���B�>5.܅])G;/C�j�K� ^  ��)����E��M�u_����CQ��]M	�(�.b0:w��8�W�U9�v�y3D��j�)g�M�B���e4�s���Lv��7nty�1�a&\�1rXEz6��\|`{M�26��}�].���A�`ɬ�����P��p��e2 wl#�oVw����%{��J{����>����ޤ_���.ǂd��`��k	w��$�{3F���Bol t�E��dT #E�j�|�u [m.N�" [�֮�@y�߬yهQ��N�ۜ ��\���)���id�����|@���:)�{~Y0��  (]I�,	5٩m^WC[M)}�if��j���?���F!�ߜ����S�~���	��A��4��8�zgU݅^���n�Լ��`����^f�բ��Y�L�?�&�J���>�H��W�.�zѐ
��0`C3t�ҕc55�GSꇎ@4���?k�N�%8�F�b��M�M煼i"�MH]t6պ�Dyo}>L<W���4�o���3��c�g@�3�A��'p��gOXM���N�h�S[�JԪ�f�I�&��Wb��Hbl�szpͰ�@[;�y�'�%c,�B��8�PB`���M�%7&�p��k. �՞�9×[e5��#4P�ݚy��(ü���c��䚤�j5����`��r��{O�����b�t�M��>�π����'����r/^���:��"�]K�;�x�ƀ�u�p�����P��;v�A����/�#�%�)j);�=���zs�o� �,͙�$@cyE#7W�/e�����P���k|a�<V-T�4d���z-5Gs�����9�oˀ�$NE�[i(��/>�����N�I�������3��      �      x������ � �     