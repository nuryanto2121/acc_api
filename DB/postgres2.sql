PGDMP     5                    x            tsm_db    11.4    11.4 }    �           0    0    ENCODING    ENCODING        SET client_encoding = 'UTF8';
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
    public       postgres    false    207   �       �          0    35640    ss_user_subportfolio 
   TABLE DATA               �   COPY public.ss_user_subportfolio (ss_user_subportfolio_id, user_id, subportfolio_id, user_input, user_edit, time_input, time_edit) FROM stdin;
    public       postgres    false    228   �      �           0    0     cm_portfolio_cm_portfolio_id_seq    SEQUENCE SET     N   SELECT pg_catalog.setval('public.cm_portfolio_cm_portfolio_id_seq', 2, true);
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
            public       postgres    false    216            �           0    0    ss_user_log_ss_user_log_id_seq    SEQUENCE SET     N   SELECT pg_catalog.setval('public.ss_user_log_ss_user_log_id_seq', 123, true);
            public       postgres    false    221            �           0    0 #   ss_user_session_user_session_id_seq    SEQUENCE SET     R   SELECT pg_catalog.setval('public.ss_user_session_user_session_id_seq', 58, true);
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
�@=�_���$S��R\�^/��"�)���כ��M^^H �ò�qX:pb��,|�&M�Ed�	 /�V� ��V�up�W޻}8Ug�hqYڻ��x2-��IZ=�4����#ɼ�`kT�p��6l�?W�/����5��O��Y'��E#�)�ш/oh ��7����M6/o�~�RNR6�z�6���5BM      �   s   x�3�,N�4�.-H-RpL����,�r����IIs�w	5��	q���,)�K)rH�M���K���t����p���%�����@��X��D���������W� �%2      �      x������ � �      �   �  x���׎������b��Ԁ/EJ�b|�(�,1<�93��x̞�����U��8y�x�(���_�/��0�;��R���L~�����߮�@�������-$~�'J,���Z�Qw�;n��o����ю?i����oc��g�W����wfGy�4����1�_SI�s�͹j�"̯�V�$������&���¥R�H�ޣ�K�Lp1�F_�����O'f�/��:d�h�Q��
H�y�Ywϕ-��2K�n�����3��Ee̫4j��|*�?V�s��tK��r#����կ������N�-� �CZ*?�zr�U�7m�N�
�)y�N��97Kzu!���2���#�lJ#=͵T�0�T�\@zH`A'^��v�H;�W���:��ǭf��K���	PZ�37�x���.;{;-j��9sGN/V�;J� Gms�A����5�!�@E"f��`|N���Bފ (>�_���g���e�_�%e�A "�8��m�o_G����� ���:�?�����ɾmWz���'��
9��y�*-���@,F(+�$g��)�PO�]U��J�V6�e�|�c�ٓ�����is{�����9�g��Zj�P�sR�L%���ŶxC˴:〚��F���ɝ{�(F�փ�'��۪j ��2��߬+S��4�tr6��''�O��;D�����e�} �!.(_;����H�-�t4|�����)��,��
��3��("ۻ��>,�Xm���K�_#*�!�,R����&���`&�i�[���]b|�A�����f��Q��$*��X+U=/-�xӨ�Z6Z��Hh�T!��d���jz&z�d��&g�Y�>����흢~�]�}Es�Kʌ��7���8C$^�w[�^���J!ʂݤ�"4x�1������B3�K(�\^^t��->�)h�Pϩ�բ/�`��<L����!����X(~�X-!~���+�fr�;mDuIМ"(V�Nw�"�s�˃ݨ�CHd����������g� �k� ����Q������_��ٜ���Q�̛��S���WdBx#�"�C��}i��N���)��&�l�њ�}#?y8�7���I/n���zA�ʕ��#�t2�$j�Tq~s/E¥���x�����'�D|�7�!ׁ(�0�T��� F������=h;/�/x�h�㷴���M�=������$���x�D��%:l�A�b�<�M/u�Nq�k��'z+�%'{���i�V�~4gzc����+�u�"��,�)qCf�3Ѓ#-��ҙVzUF��.� e�b��Y.���f�]�i�ՙ6�L�����lmGU(s��i��I���w����{���/���~^D~`�ɵ�Zf
� n�<J2��m���ٸ0�<Y�u��J���������h�O�=��6 �w�xM��Sp�Aײ0.d��3閼��"m�R����(��z9�nl��r �r���U�W�,��N�X�E"�R�$S�G��1OR���M�g���������"�`�>.^E++G�gCBU�sN����ކ(����(��񢕺�RJ>Uи$������b��&��JF��6�K[�YM����^�=��k1@*�4��.��g���9/�Ð2������|\�7ܦ��^B;m�?��:��7�5�,]�I��}���m��y˼�����r��a����4�ץ a�Y�$h)��m�����z�d2�z�cd��3�rcVO8�'ȜD�ptLK�j����R��R�� �g��0���lC	 �k}��,���XRnB�k����I�[�#��Eo�r���!�퍢����pu1�^��|���~�%�xQ����D&�To�l�yc���
�=s\%}�7 `R��A�Q�B�e!�����HkC1_�.��j9����(	D���@I�T�P�m��C��m�R��_,�U@����a�"赃^=��>.�*"� ��"+����5�5�Tb�������|����>�c^Xb ���:q��jp�|L���"���kF��Nr߈��D�A��W<��Z��hF�(s�U7�a�7%��p?�F8�H��pb��^�p�Գ%� �/q*#�]�i���:V�Lܥ�����+�V��b�eTJ2���ڛ@��YMQ�E�ȯ��������u���=t�P7�%S7�l���i�,�u�w�L����}��������v�#9}M�fR�-�9v�;CP�V$���/�qǤ���5�A�O��>�Ky����=�R9{Wn�3=��yA
E��&�r�T[u�����Rۼ�S�-F�缾{>��!|K��=��!�7�'ݏ}v;�ʦ��w�:��@�cB���Zw�ӡ��+��3�	�{ꕂĒͦS�R1���X�r�zT��H2s�sט��	h�^{���Y�g3v�VͣM���S��ʕ���z_g����ne�����y�!�+K�xׯ����4���˵��5�Qfx�0�]��+�=<�<h;/�?��_�����5�?��wh���yq���bG���8�7�_#�D��;�=�_D;�c͔e����ҋ�0�j�/���2�{9r�9��$��e��Y���\./�"�[��ӇWz)��i��a�9W�x�6Y.\��&JY2,�n�a�5gnJ�����欤4�58Ɂ)�4s��c���1�W$�V0�������sz\Y�|���~;~8E�kG���qR�v&>t�1��.�-l����Q����Iă��������5��N��NBb���>O�\�L����ʕ���N£�f��rIj�[���ܝRw�K~i}E��U4�nY^���q&�����:)#f�3�>|7��$;���(5\s��j\u��������/���� ����      �      x���Ɏ�ʶ�����O�ͺ2p$E��X�+��u]�O���V:Ӿ�{�|�x͈���Y%��e�/*�E���d��EJJ�Ҙ�>�)'*
h�VY^i={�pVEƼc��Q��ĺ��3��88gl�}��\��x���P����PX+�8G��Ae�G�n�nR�J�8.��C�쭲r�i�r���FSݹp7�
W�WK����!��@��@����o��>������w���E��߱7�o��m7�|j2Z��D1�r�@�|/�4�;}[���Va�y�-LX�񅮍�2�6�X���"�sg<[�P�HvkD�B�;�"i�:��A0$���w�)�x~'�#��*���I�2�(t}�~��>� i;T��~ٺb�3bc���g��Z�#���b�b��~�ް�I\�z�p��ψz�Mu��%�4��WHdn���րHE��0c+b�Ore���������m3���PGs��tpu�p���Yoq#axE�Ck��������E�L��e"���3,);��'9^L��rbTo�v�e�BEk{-ǟ���o�F>�������ϯ�A�����@з�������T����s��K���������j$�YUt����Y���&g$.4��q	 �v��9Q�lm���m��A��*D����-Lʙ�P� &՝��	�yk���s5�|���Ny�K���,�A�ĸ�V�����0E4�!�vG[���?��WӨo(�մw��6�W�`o��
O����D�q��N�&��H���<�ڕ2�n�j�W��%'�Hb@��+�n	!��-��s4g.Ŵ��T#��ue�3�	�d�y��`˒U���pO!�5O��bDi��K�M���)�H ����Ǭ��)�v�h*ρ�S&	8�e5_ʃ�o8�Ŵ�?5��
��[Q��O��l6E�ڎ� ��dm���x<e�^�j>�r����H{��ö�>��i'?�$1k*J����v�ek(�B0�-�.N}$��~���JǕ�����F��"�B��1޶D���2� 7ɥS����%�3#Cb9&;���	�*�E�������#��W�������>����?���#f<������A�����uĥ�uƱMm$dR �
�u��u�Z�R���TjX���m�n� �Vawr����]6��_E�I10�0+�}�c5
i�0����~�k�\:�h`|�5ʏ��:-y,��gz�5�gy�&�wn�A��ܨ��db�C�{s�5���_�|1�?7��
��[�>6������y��FM��#ҶK��=nT$?[��GꉃXsr�՘(g�3D4Z"椌j�t��P��[H���r��u%� ��z�l�]P�|7�s���nYµ�ZY��<�����kޮ�E;PRݵ5�ݠ�n�^Q{��lDC�o�Oт�`�x��N���`�oIqz7�u��!�Ǟ�S7��X��2�$)��m
c�� ���
2���z^��uiN��d�"�F����dZ3�jS/�m��Ke��W�s&���M��紲�b�1��Y�`� �h-�j��h�e<Kġf���;1{�l|x�%̠�թ��#�qd2���N�!&~�w�a?����Q��|���i��0A���7N���/�|�7����X��2����Ś�~0TN�\���bu��N>�g��c^�*{�ytJ��(F��}���+��5ss.���c|&�y뀺�'ZKǼ��2�uz��z8�V/�|��
^Kmn2�ĞǙ�]�}�g$o�!�ckC���e��q�<*`����;ȳ"�g�����b��_�e��$���_w���"�`��a8.��R�3�t��1#iT�z�qX����;9�XY�،�ju��l=���1�qg���R4���ԕaI��֛{Y��#�יV��z��pZ^n���0L����f�e��$��uv�6�����*��胳�k�0	iJ덖*!��b(N�����0��"�����]3��w�õx�{E�tGW[�Ŷ
I	��E7W���� *�<@a"ޙ}/V+�1C���B7I"۹bC����al����Œ�RKN�(>#V�T��a���~�Q:�B�PlR]�K�;�y[�]�hӉ�[�vl����a��"vX��\�q��ćʑ�ܳ��l(�������'C�f����.�����a��l�e��gZwH���t\,r�6��Nk+ȷ�P�z.��}_�K���G�5�/V=kH�{��L�����23u��b[��4ְ.�<���#炔!&�CB���}%L�0���ޱ̑����S�5ʥ�1�� �(��ei�|Q�E?r0c��Z�e�=W�sr�H�~c(��m����C?/�����AF��B|�Sal,ju,�m�n�&���G�!tGd�6�}��`�R��V���r�=m�r�n��G1�6VO�8��BFS�	:�W��k7ݕ }Y �i(O����;R�LT#+F�_�'jp�����i�L�;|�p�Wy��I�*'�UV#
C�Qe&� ��!��1�5�P_}g�����`o������J�5 �ޖ�[-�s:���բ��GZ�zex����9��|�=�獌�}=	 �Z�ȳW��%�Q(��	�A:/���vy�*e��'��eۙ��u�ǔ�=�OY�3�ir��R�zFLvee��ݝ���A5��'c��`�����	ג��iG8ɂ+�WC�o��ܡ����_�`o�� dl�����C"�
��Z�#�̍��W"���d��ש�.�EbMp�����C��������O�5�U�c���LĘ��('x��QwG~�/I��}�bױ��l]�a�2�jΧ����k����&U��W{�E��xm��:\1ns��{�$\惆������b(��1���C�,���`�_s��� ��m��Q^�FH{ �}.�C�7�Y��jz�
U��ಂ殔�NB_�n���SCw���	-��j��|��j�<��R�Fu@3�dY��ĕ�g10��Ȇ��Fr%��$dikX9'+��yV�c��1��&wCD@r8F��%V	���4��c�/���O��
��Q���^g���v'z��Ǹވ��}թ��fISieߘ<�P�Pm��ؽ7,���#ZϞX*W~%�x��dr�G˽6��dέJMK�X���-��l�R�ᚦ�Y����&8����íF�č~�=U(#>z��IӺ�OO�f�C�����L�p������W8��:�����g�{C���kPM6fV̋�����d$��>�.Qi+1uwpD�	��(c{��7&(w�^/�I��}�9�FN���?I���%��C�t'������.=�n慕8��v��|���"�Sx�8窹w`۠k�2>��v���u䷱R��l�6'���p5RV�/���6�kO�`t��!��w��㱓n�VW|�@���M>�u`��|���u�>:N(�/��g`�X�a"�ҥ#i��]�ܮ���ϺN0��Y���#��E� ���U����lܝ��Z%Q<
���0��~�>m6�G#=6����SX`]M����ki��W2���;��S����e��݁��9���!��%��x)(zgfz���TD\�2��>���{�G{��H
��)�T⤒Ob��c�
�4e���~��W"r�����.&Z�j��^
2$,M ���]��6��
�h�#����IF�U�����ڢ��"?Ãl�Ki�����������{C���f���%�cA	S�j���b���yp��>˵��J�[gT��mP�]�!�q�(o�N`4�J��%B)t�
t.*�Cѳǂ��kDߋ.�r���u&m2xN�<i����b��<ғ�G��5K�&�#�$�|V}�(�|l��҈o��s���S�����O��B��#B�׊�z�1l�I���W������u-�B���S��^�����ꖞq�1]�	�'aH�L�I�x@ͣ���F9�<*Y΀1��_�k�:��`}ͮR    |� Pc5���ł)3�bf���X��N�	����4�������3]�A�Z��ǝ���S�<k�_���ﻏ>�Lp&�U�u�9�J4ΐ����o
���@�Ԙ�I��dZ/bN���V��F"Zwg]�v��HZ6-��fO�Q�@�p�iET�Y7��x59d��|D�T��.�:��3
�9�K+�ml�K�<Z���[�|�Wk6s��s���v.���_�*�kT�8�K@}goȻ �A'����@�_W&i�1Ͱ1�K�u(�<�I>��2LF��l�4�Q.�@;g��b�^�7v؇lK�>e6�l�e n��^�7��.�a�I�n<�6m���
��B3�ژ-`>Ӿaf�$\g�;��(g*G��3~��t�/�a��o�>;���ѩ/�����)�.���3���D=��=��`�������:�&��,	�V�~�}U��O3���"�05��������-���e�IY�D�d&l�7v��tS���g⻲�K6BK�,�D��g�
?��0
A�.��vl��2��L�Si�� }�'�b��j�/����Gsٮ�=\l��e�jr��뙵���C&s�N���`��G��B�����r�>�8�a�=�)v|���r���'��_;>�|�<-��m'p��d �#j� ǔI�mPR�з�3�@�Y��A�iY`W�#�cn~�-��(�f��Fk'�w��6I���� ��A?�G+��o��"�`o�{4�Ĺr��%xH�8΋�R3A�x�]�>�����Y�/J�:��t���U�t�U���-S�W��b�fHa��\v�y�:��hćg+^`���ιgtm��E*�u��_�̠m�`��lE\OQkU-nd��&���m����~�F�O@���5}�����:��O�}��%l�6ڍ�r��Ү��xX���[�-�_-��\v;�!}�[״&\�Ƙ���Ȧwo�sy\�uF�p0?z(uKr��l���
u�L��O�1 d���ر �=OA�����L�P2?)���1�WJ��5,F񸰔/�}�=��~�:�~#������U��Ujg��y�����V�00\�kwd77�H�� 9cUў�q���k*)rS���uBCܾ?O��D�	�	�>�IA�����	�R��������Z��z)�ޫc�hj!F��/F��!e��t�����y�w��X��9�_K�~>l�שw�Ǟ�,�����ќt�q?o�	�#g,X�J�H���z��'��ڬP7(��^���.�i�}KQ䡞�&��s�?_Հ�/��2���[���.�zO8�N�P�d��v<l���-.`K�*ݢ}���N���f���)��M3��ٳ�4���RҲdLz����`�{B��������S�Hbo�{4Oɕ*���Օə�{�X����;�XDV�C���RD���<�>���Ffh�2H7Gժp�C�2yr۩r>R��U3�wq�N�_n�Z�����p�z�:4��fE(|�֮��Hൈ���j��		r�^��Z2�K��F��i�Lܟ���J����7���������E�{Cߣ9\>�}5��1g!X���r�"�c��,���Z\S}�8��H�Zй�෨ƅ�b*m���0�אp��x�=��"��q���0�e�x<���s,�<Rx��M�rg�uZ�*f�?�+�;]I$Y�c6B-�R3˅�J�^��Si�S�rN��������'���Gs]Q� 3S��x$�����f�[a�[tf�����ϲq�?23.�B���bA��]�I��}���yr�BJ�Ηuq�x��H(�S���7E���>�q��P.al�S+e����m�����'7�p�\��� w����6�8�\��Ki��u��CO}�����wAp>��n���$-dNl�G.+�ӻٝ�1���io�~�'�'�YK�x�M�a��j]5g�n21h]�ǔQ���R1~X�(C[�e��$Uм"L52�m/H�,��P�8L%��c�d�I���n��ZnB���� G�ĵO�2���/���7����G�>Kbo�{���h
���0ۙHQ�V4*��3�x�ݟx�$��Qp�ӥ�u�?��5�vLb��7dw����L2�t����!H���5���K	"O��^��ڠ�M{ꔏ���u��=�nF�n+�Ģb2�?���Np˫I6if�z��"����z0|*�^}r�/���Jbo�� <�(���`Ým���_z)$�m��MXm�z|�Ay̩�i,����GZjGw���hBk�.+r�]b-�³��
��XG2��ne��n��5�d֞���Nӂ^m��V�c����
1���łgR�������;��y,�_�]+����4��wW������S_�?�7�=�7���,����Aݣ'!�
��W�3dC@�ԝ��Red"�/,���)v;!t�![���qun�q��	nHi�]��z�
�$Ad�e�
/�$�Rǻ��8��h�0�TDQ}�X8����;�{O�kE�l�[l�gV�ʚ�{vw��R����/N�`t��'���gt���¢�+ً�1��m�G���T��uA�l�BU1�xg��u����(��Hu]�;�E1�Ɨti�(��.�ؒmH�Ԛ�ƥт���&��ƴ�\`��ð^} t%�ç0�	�_fd��l��S\'69{.�[F�z�����o8�թw�g�>Ibo�{F��c���r�F'vQ�)�7o�dS��e���?���48���"@�e�Q�����y)i���Q��Z�>�U�����P���S�m�1ڭ*G!bVjHbppVOZ�4�֮���_�Kdv����>�[�u�f11������屯��� �0�S?���,���a�=5��"���<.�)�4�͂�{/���軞y�Ww�#l�)d��.Y�ʁW��=�3�*�Lwi
xր'}�"�����TUՅ�V����t������2�E�oNP�G�>`�B��(�D'��Ԅ!!b�����fX��~�I��Tud���Ҩ����N��?;�I�{��3�_a�a�#~>Y�TÕ�Dщ��K��C��i��\�P�l�>_������F�]�����aeD�#]4y3�g	gqb��zC�N��vO�;VB[y=mV�6Z��j���w캭q6��^S�q�5�ث	��uM�W�� WM��'��tښ��ݔ��}}��7��S_�%��ް��.f�gڝaO"�n$��)�G�7n�Pb *��{N�V�"<��
���º�=�㐇#+�R9R̃Q�͋�[�3�A6��r�V�Œ�B��i
�#�V���c"V���O[H6��Kխ�.'��)V�$��3NR%c)]	ezt��Uܧ�^�
�����N���0�}���ް��.���+����%]��m�Rx+�V�-�÷؂�!A_Ab�a�*7��K����`U�D����Y�Y�������WJ#�{���M<�0ƔF+Q��9�eS���r�^��y��,����P-��yӚJ��w��2�p2i&���)�>���u�	�`���o��{o�2^۰hj������s�-���,�������B �H�b�+��k�����y׬z�lH�\�d�/0Tw9���^�L�W۰��:ӛ�7ۉT���=t�N\��r�$�*q���$�(�K�O'Q3
W�bZ�H��\w�%!}g	��~>�|��}��~�7�=�+���'�t��Nht���zj ��1!&쪎X&��39�=i��\�����m��2�:1˃Y�����7��C�
��QM�ů�͌8�7y���Β�� �am�Ū_�f㴨a�u���� ��5��\���H��j���4��|u�����H?��.8�z�Jٕ^A�e�=Ze0hSV�ڹ֭��:k�Q�.�V���
�^����G�:^��2��J���+S�*��)�~�>��X�TnC�F+�wG����f�΅�C	AMI�GS�'jU��ȔD��M��Z)�ЪG�waWJ���M�5�ꯥ �  Q������E�֦Ϻ�����WT��`WS�4�ˋ菹�ݧ8J��tTθ𢝹��:F�dʩ|�y?Wm�eo�g
�OɌ��w��۬h�0΃�9�$]�3Ea.���o���f�x�?�h�`��u۹�O��I<�
j�e2�;��G�ۭ�rC�I��})�~~���ǆ�"�`o�{@w8� �� C�m-H�s�%	>�1��S-���B{\*�G2�,̳^���<��#���`����͚�c�6i���xz\q���P\Թ��:Z �$z��`�m�NJ�]L�� %+����&;U���j`��)%��f@?�����si�7
�����쟝�U�����&��]2Լ�� �;����p|u��f9uC����2�X��m+�g1Q�=�Ўq(��>�c>�xM/�C���&���g��f�ڡˇUWN�<D �I��mo�`C����!��M^;�6���h;!U��T����:޺,�M<S�~S�����)�w��xj������:ª�%�uJDK�ړW�V'NR6��p���Gbc7���(�a���Bs�;NxK�X΅��q����P)��K�L:4� #W_@b�#�Ds�/�Ҫ�Ch���5u��P�y_g�d���q-)�j mS�����0���=��c��ѩ/�������o�o`�=����"��}�yO�an�H��i�wl�9[GrJ����#9cK$n��={9>�cAН����;�����ҜIO�Ԗ��r}E�Bv����{��vY��*��J�����UB�����I��v�^��e��B�Ki(���~8���ѩ/�����H�^㽴)W�"�.0���=V,@��R���k8�q��I���U��u�,|�f��z=�臵f�V⎽VƧ�+���<�O�[��!����NS��X������7��90n���^�:����VG,�A&�M *j��<�&��k���>��\!��l��_������迕~�7�=�3i��s�ToL��=�_�ïϒh��j_�,�=�{
S�hY�E���n!�k�=d�I��8����j~<	[�n�����k�Sx2�v/�r���Z��v�h�e�"�̬x����y���V�����.Z�f�CChEX�?䠩' q!��
��0���)v�N���~Lv,S�l�.�|����ˑ�l����S�� ����K���t�6xE��eC����&��ݘz����e460�*�7_�czl�!m�Z��l)��wW��������W�#�s�1[1�h�K镺"�p�A�|ڇ�D���A�Tګ�O?e���_�N�"�`o��&lٺ�ƾ��Q{I�p���3q���Q�{/7��0-����Q��O�7���{��f?��2J/�vxDI?�PҐ�|�zȗ�9�.�zxi�$����tޏ@�:�2��r}�~NR;�JhI-���q�����	-��W����y���t����B�!?6��i��_������R��bnp�U!�F&¹��N΢����1aJ����&��Ȋ��ы���9D���K���Z�O�)�OJ�'����v��KI��{��> �{�����0�s-b8��R&)�>�Q4'cłr�
y4�z��#���,u�^z��\m�v�{���OA��`�tN�N�����������g�      �      x������ � �     