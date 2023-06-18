alter table course_taught drop foreign key FK_COURSE_T_COURSE_TA_TEACHER;
alter table course_taught drop foreign key FK_COURSE_T_COURSE_TA_CLASS;
alter table project_undertaken drop foreign key FK_PROJECT__PROJECT_U_TEACHER;
alter table project_undertaken drop foreign key FK_PROJECT__PROJECT_U_PROJECT;
alter table publish_paper drop foreign key FK_PUBLISH__PUBLISH_P_TEACHER;
alter table publish_paper drop foreign key FK_PUBLISH__PUBLISH_P_PAPER;

drop table if exists course;
drop table if exists course_taught;
drop table if exists paper;
drop table if exists project;
drop table if exists project_undertaken;
drop table if exists publish_paper;
drop table if exists teacher;

/*==============================================================*/
/* Table: course                                                 */
/*==============================================================*/
create table course
(
   cid                  char(255) not null,
   cname                char(255),
   chour                int,
   cnature              int,
   primary key (cid)
);

/*==============================================================*/
/* Table: course_taught                                         */
/*==============================================================*/
create table course_taught
(
   tid                  char(5) not null,
   cid                  char(255) not null,
   tyear                int,
   tterm                int,
   thour                int,
   primary key (tid, cid)
);

/*==============================================================*/
/* Table: paper                                                 */
/*==============================================================*/
create table paper
(
   pid                  int not null,
   pname                char(255),
   psource              char(255),
   pyear                date,
   ptype                int,
   level                int,
   primary key (pid)
);

/*==============================================================*/
/* Table: project                                               */
/*==============================================================*/
create table project
(
   jid                  char(255) not null,
   jname                char(255),
   jsource              char(255),
   jtype                int comment '1-国家级项目，2-省部级项目，3-市厅级项目，4-企业合作项目，5-其它类型项目',
   jbudgets             float,
   styear               int,
   edyear               int,
   primary key (jid)
);
alter table project add constraint check_years check (styear <= edyear);

/*==============================================================*/
/* Table: project_undertaken                                    */
/*==============================================================*/
create table project_undertaken
(
   tid                  char(5) not null,
   jid                  char(255) not null,
   jtrank               int,
   jtbudget             float,
   primary key (tid, jid)
);

/*==============================================================*/
/* Table: publish_paper                                         */
/*==============================================================*/
create table publish_paper
(
   tid                  char(5) not null,
   pid                  int not null,
   ptrank               int,
   correspond           bool,
   primary key (tid, pid)
);

/*==============================================================*/
/* Table: teacher                                               */
/*==============================================================*/
create table teacher
(
   tid                  char(5) not null,
   tname                char(255),
   gender               int comment '1-男，2-女',
   title                int,
   primary key (tid)
);

alter table course_taught add constraint FK_COURSE_T_COURSE_TA_TEACHER foreign key (tid)
      references teacher (tid) on delete restrict on update restrict;

alter table course_taught add constraint FK_COURSE_T_COURSE_TA_CLASS foreign key (cid)
      references course (cid) on delete restrict on update restrict;

alter table project_undertaken add constraint FK_PROJECT__PROJECT_U_TEACHER foreign key (tid)
      references teacher (tid) on delete restrict on update restrict;

alter table project_undertaken add constraint FK_PROJECT__PROJECT_U_PROJECT foreign key (jid)
      references project (jid) on delete restrict on update restrict;

alter table publish_paper add constraint FK_PUBLISH__PUBLISH_P_TEACHER foreign key (tid)
      references teacher (tid) on delete restrict on update restrict;

alter table publish_paper add constraint FK_PUBLISH__PUBLISH_P_PAPER foreign key (pid)
      references paper (pid) on delete restrict on update restrict;


-- # ---------- VIEWS ----------
drop view if exists project_details;
create view project_details as
select project.jid, project_undertaken.tid, project_undertaken.jtrank, project_undertaken.jtbudget,
       project.jname, project.jsource, project.jtype, project.jbudgets, project.styear, project.edyear
from project_undertaken
     inner join project
     on project_undertaken.jid = project.jid;

drop view if exists paper_details;
create view paper_details as
select paper.pid, publish_paper.tid, publish_paper.ptrank, publish_paper.correspond,
       paper.pname, paper.psource, paper.pyear, paper.ptype, paper.level
from publish_paper
     inner join paper
     on publish_paper.pid = paper.pid;

drop view if exists course_details;
create view course_details as
select course.cid, course_taught.tid, course_taught.tyear, course_taught.tterm, course_taught.thour,
       course.cname, course.chour, course.cnature
from course_taught
     inner join course
     on course_taught.cid = course.cid;


# ---------- PROCEDURES ----------
drop procedure if exists paper_del;
delimiter //
create procedure paper_del(in pid int, out state int)
begin
    declare s int default 0;
    declare continue handler for sqlexception set s = 1;
    start transaction;
    if not exists(select * from paper where paper.pid = pid) then
        set s = 2;
    end if;
    if s = 0 then
        set state = 0;
        delete from publish_paper where publish_paper.pid = pid;
        delete from paper where paper.pid = pid;
        commit;
    else
        set state = s;
        rollback;
    end if;
end //
delimiter ;

drop procedure if exists teacher_del;
delimiter //
create procedure teacher_del(in tid int, out state int)
begin
    declare s int default 0;
    declare continue handler for sqlexception set s = 1;
    start transaction;
    if not exists(select * from teacher where teacher.tid = tid) then
        set s = 2;
    end if;
    if s = 0 then
        set state = 0;
        delete from publish_paper where publish_paper.tid = tid;
        delete from course_taught where course_taught.tid = tid;
        delete from project_undertaken where project_undertaken.tid = tid;
        delete from teacher where teacher.tid = tid;
        commit;
    else
        set state = s;
        rollback;
    end if;
end //
delimiter ;

drop procedure if exists publish_paper_insert;
delimiter //
create procedure publish_paper_insert(in tid char(5), in pid int, in ptrank int, in correspond bool, out state int)
begin
    declare s int default 0;
    declare continue handler for sqlexception set s = 1;
    -- start transaction;
    if not exists(select * from teacher where teacher.tid = tid) then
        set s = 2;
    end if;
    if not exists(select * from paper where paper.pid = pid) then
        set s = 3;
    end if;
    if exists(select * from publish_paper where publish_paper.pid = pid and publish_paper.ptrank = ptrank) then
        set s = 4;
    end if;
    if exists(select * from publish_paper where publish_paper.pid = pid and publish_paper.tid = tid) then
        -- 主键重复
        set s = 5;
    end if;
    if correspond = true and exists(select * from publish_paper where publish_paper.pid = pid and publish_paper.correspond = true) then
        set s = 6;
    end if;
    if s = 0 then
        set state = 0;
        insert into publish_paper value(tid, pid, ptrank, correspond);
        -- commit;
    else
        set state = s;
        -- rollback;
    end if;
end //
delimiter ;

-- drop procedure if exists publish_paper_del;
-- delimiter //
-- create procedure publish_paper_del(in pid int, in tid char(5), out state int)
-- begin
--     declare s int default 0;
--     declare continue handler for sqlexception set s = 1;
--     start transaction;
--     if exists(select * from publish_paper where publish_paper.pid = pid and publish_paper.tid = tid) then
--         set s = 2;
--     end if;
--     if s = 0 then
--         set state = 0;
--         delete from publish_paper where publish_paper.pid = pid and publish_paper.tid = tid;
--         commit;
--     else
--         set state = s;
--         rollback;
--     end if;
-- end //
-- delimiter ;

drop procedure if exists project_undertaken_insert;
delimiter //
create procedure project_undertaken_insert(in tid char(5), in jid char(255), in jtrank int, in jtbudget float, out state int)
begin
    declare s int default 0;
    declare continue handler for sqlexception set s = 1;
    -- start transaction;
    if not exists(select * from teacher where teacher.tid = tid) then
        set s = 2;
    end if;
    if not exists(select * from project where project.jid = jid) then
        set s = 3;
    end if;
    if exists(select * from project_undertaken where project_undertaken.tid = tid and project_undertaken.jtrank = jtrank) then
        set s = 4;
    end if;
    if exists(select * from project_undertaken where project_undertaken.tid = tid and project_undertaken.jid = jid) then
        -- 主键重复
        set s = 5;
    end if;
    if s = 0 then
        set state = 0;
        insert into project_undertaken value(tid, jid, jtrank, jtbudget);
        -- commit;
    else
        set state = s;
        -- rollback;
    end if;
end //
delimiter ;

drop procedure if exists project_del;
delimiter //
create procedure project_del(in jid int, out state int)
begin
    declare s int default 0;
    declare continue handler for sqlexception set s = 1;
    start transaction;
    if not exists(select * from project where project.jid = jid) then
        set s = 2;
    end if;
    if s = 0 then
        set state = 0;
        delete from project_undertaken where project_undertaken.jid = jid;
        delete from project where project.jid = jid;
        commit;
    else
        set state = s;
        rollback;
    end if;
end //
delimiter ;

drop procedure if exists class_del;
delimiter //
create procedure class_del(in cid char(255), out state int)
begin
    declare s int default 0;
    declare continue handler for sqlexception set s = 1;
    start transaction;
    if not exists(select * from course where course.cid = cid) then
        set s = 2;
    end if;
    if s = 0 then
        set state = 0;
        delete from course where course.cid = cid;
        delete from course_taught where course_taught.cid = cid;
        commit;
    else
        set state = s;
        rollback;
    end if;
end //
delimiter ;

drop procedure if exists course_taught_insert;
delimiter //
create procedure course_taught_insert(in tid char(5), in cid char(255), in tyear int, in tterm int, in thour int, out state int)
begin
    declare s int default 0;
    declare continue handler for sqlexception set s = 1;
    -- start transaction;
    if not exists(select * from course where course.cid = cid) then
        set s = 2;
    end if;
    if not exists(select * from teacher where teacher.tid = tid) then
        set s = 3;
    end if;
    if exists(select * from course_taught where course_taught.tid = tid and course_taught.cid = cid) then
        -- 主键重复
        set s = 4;
    end if;
    if s = 0 then
        set state = 0;
        insert into course_taught value(tid, cid, tyear, tterm, thour);
        -- commit;
    else
        set state = s;
        -- rollback;
    end if;
end //
delimiter ;
