PGDMP         )                w            core_db    11.4    11.4     ]           0    0    ENCODING    ENCODING        SET client_encoding = 'UTF8';
                       false            ^           0    0 
   STDSTRINGS 
   STDSTRINGS     (   SET standard_conforming_strings = 'on';
                       false            _           0    0 
   SEARCHPATH 
   SEARCHPATH     8   SELECT pg_catalog.set_config('search_path', '', false);
                       false            `           1262    48305    core_db    DATABASE     �   CREATE DATABASE core_db WITH TEMPLATE = template0 ENCODING = 'UTF8' LC_COLLATE = 'English_United States.1252' LC_CTYPE = 'English_United States.1252';
    DROP DATABASE core_db;
             postgres    false            a           0    0    core_db    DATABASE PROPERTIES     1   ALTER DATABASE core_db CONNECTION LIMIT = 10000;
                  postgres    false    2912            N          0    48308    sys_menu 
   TABLE DATA               �   COPY public.sys_menu (sys_menu_id, title, url, parent_menu_id, parent_menu_title, icon_class, path, order_seq, user_input, user_edit, time_input, time_edit) FROM stdin;
    public       postgres    false    197   I       P          0    48340    sys_menu_rigth 
   TABLE DATA               �   COPY public.sys_menu_rigth (sys_menu_right_id, sys_menu_id, sys_role_id, add_status, edit_status, delete_status, view_status, post_status, user_input, user_edit, time_input, time_edit) FROM stdin;
    public       postgres    false    199   f       R          0    48355    sys_role 
   TABLE DATA               h   COPY public.sys_role (sys_role_id, role_name, user_input, user_edit, time_input, time_edit) FROM stdin;
    public       postgres    false    201   �       T          0    48367    sys_user 
   TABLE DATA               �   COPY public.sys_user (sys_user_id, user_id, user_name, email, handphone, is_active, pwd, company_id, picture_path, user_input, user_edit, time_input, time_edit) FROM stdin;
    public       postgres    false    203   �       V          0    48382    sys_user_log 
   TABLE DATA               �   COPY public.sys_user_log (sys_user_log_id, user_id, ip_address, login_date, logout_date, token, user_input, user_edit, time_input, time_edit) FROM stdin;
    public       postgres    false    205   3       X          0    48396    sys_user_role 
   TABLE DATA               k   COPY public.sys_user_role (sys_user_role_id, sys_user_id, sys_role_id, user_input, time_input) FROM stdin;
    public       postgres    false    207   P       Z          0    48443    sys_user_session 
   TABLE DATA               �   COPY public.sys_user_session (sys_user_session_id, user_id, token, last_login, expire_on, ip_address, user_input, user_edit, time_input, time_edit) FROM stdin;
    public       postgres    false    209   m       i           0    0 $   sys_menu_rigth_sys_menu_right_id_seq    SEQUENCE SET     S   SELECT pg_catalog.setval('public.sys_menu_rigth_sys_menu_right_id_seq', 1, false);
            public       postgres    false    198            j           0    0    sys_menu_sys_menu_id_seq    SEQUENCE SET     G   SELECT pg_catalog.setval('public.sys_menu_sys_menu_id_seq', 1, false);
            public       postgres    false    196            k           0    0    sys_role_sys_role_id_seq    SEQUENCE SET     G   SELECT pg_catalog.setval('public.sys_role_sys_role_id_seq', 1, false);
            public       postgres    false    200            l           0    0     sys_user_log_sys_user_log_id_seq    SEQUENCE SET     O   SELECT pg_catalog.setval('public.sys_user_log_sys_user_log_id_seq', 1, false);
            public       postgres    false    204            m           0    0 "   sys_user_role_sys_user_role_id_seq    SEQUENCE SET     Q   SELECT pg_catalog.setval('public.sys_user_role_sys_user_role_id_seq', 1, false);
            public       postgres    false    206            n           0    0 (   sys_user_session_sys_user_session_id_seq    SEQUENCE SET     V   SELECT pg_catalog.setval('public.sys_user_session_sys_user_session_id_seq', 1, true);
            public       postgres    false    208            o           0    0    sys_user_sys_user_id_seq    SEQUENCE SET     F   SELECT pg_catalog.setval('public.sys_user_sys_user_id_seq', 3, true);
            public       postgres    false    202            N      x������ � �      P      x������ � �      R      x������ � �      T   �   x����
�0E����?В���ɂ�E]jK�����%�[]΁�2�m�p�s�fn����\�X�0@����+1��,��;o�Wj$8�vx�:����Rq��m_jd�M:�y����+5:7D��E�      V      x������ � �      X      x������ � �      Z   �   x�m��n�@�5<E��v~R q!�L��8��7 R'ܑt�O�mL���9�/9����$�%8���PH��ͶK�����(�����Ѵ1�a0�n�n�>B����ć3�FL�ȃ�{Zkf�:�?�s���	m����bg��1u;1S��h`��#��-��{�q��N%�9��'���[:��v7�$˂�#?������e�
F�     