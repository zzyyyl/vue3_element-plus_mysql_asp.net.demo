/*==============================================================*/
/* DBMS name:      MySQL 5.0                                    */
/* Created on:     2023/6/12 15:02:32                           */
/*==============================================================*/


alter table course_taught 
   drop foreign key FK_COURSE_T_COURSE_TA_TEACHER;

alter table course_taught 
   drop foreign key FK_COURSE_T_COURSE_TA_CLASS;

alter table project_undertaken 
   drop foreign key FK_PROJECT__PROJECT_U_TEACHER;

alter table project_undertaken 
   drop foreign key FK_PROJECT__PROJECT_U_PROJECT;

alter table publish_paper 
   drop foreign key FK_PUBLISH__PUBLISH_P_TEACHER;

alter table publish_paper 
   drop foreign key FK_PUBLISH__PUBLISH_P_PAPER;

drop table if exists class;


alter table course_taught 
   drop foreign key FK_COURSE_T_COURSE_TA_TEACHER;

alter table course_taught 
   drop foreign key FK_COURSE_T_COURSE_TA_CLASS;

drop table if exists course_taught;

drop table if exists paper;

drop table if exists project;


alter table project_undertaken 
   drop foreign key FK_PROJECT__PROJECT_U_TEACHER;

alter table project_undertaken 
   drop foreign key FK_PROJECT__PROJECT_U_PROJECT;

drop table if exists project_undertaken;


alter table publish_paper 
   drop foreign key FK_PUBLISH__PUBLISH_P_TEACHER;

alter table publish_paper 
   drop foreign key FK_PUBLISH__PUBLISH_P_PAPER;

drop table if exists publish_paper;

drop table if exists teacher;

/*==============================================================*/
/* Table: class                                                 */
/*==============================================================*/
create table class
(
   cid                  char(255) not null  comment '',
   cname                char(255)  comment '',
   chour                int  comment '',
   cnature              int  comment '',
   primary key (cid)
);

/*==============================================================*/
/* Table: course_taught                                         */
/*==============================================================*/
create table course_taught
(
   tid                  char(5) not null  comment '',
   cid                  char(255) not null  comment '',
   tyear                int  comment '',
   tterm                int  comment '',
   thour                int  comment '',
   primary key (tid, cid)
);

/*==============================================================*/
/* Table: paper                                                 */
/*==============================================================*/
create table paper
(
   pid                  int not null  comment '',
   pname                char(255)  comment '',
   psource              char(255)  comment '',
   pyear                date  comment '',
   ptype                int  comment '',
   level                int  comment '',
   primary key (pid)
);

/*==============================================================*/
/* Table: project                                               */
/*==============================================================*/
create table project
(
   jid                  char(255) not null  comment '',
   jname                char(255)  comment '',
   jsource              char(255)  comment '',
   jtype                int  comment '',
   jbudgets             float  comment '',
   styear               int  comment '',
   edyear               int  comment '',
   primary key (jid)
);

/*==============================================================*/
/* Table: project_undertaken                                    */
/*==============================================================*/
create table project_undertaken
(
   tid                  char(5) not null  comment '',
   jid                  char(255) not null  comment '',
   jtrank               int  comment '',
   jtbudget             int  comment '',
   primary key (tid, jid)
);

/*==============================================================*/
/* Table: publish_paper                                         */
/*==============================================================*/
create table publish_paper
(
   tid                  char(5) not null  comment '',
   pid                  int not null  comment '',
   ptrank               int  comment '',
   correspond           bool  comment '',
   primary key (tid, pid)
);

/*==============================================================*/
/* Table: teacher                                               */
/*==============================================================*/
create table teacher
(
   tid                  char(5) not null  comment '',
   tname                char(255)  comment '',
   gender               int  comment '',
   title                int  comment '',
   primary key (tid)
);

alter table course_taught add constraint FK_COURSE_T_COURSE_TA_TEACHER foreign key (tid)
      references teacher (tid) on delete restrict on update restrict;

alter table course_taught add constraint FK_COURSE_T_COURSE_TA_CLASS foreign key (cid)
      references class (cid) on delete restrict on update restrict;

alter table project_undertaken add constraint FK_PROJECT__PROJECT_U_TEACHER foreign key (tid)
      references teacher (tid) on delete restrict on update restrict;

alter table project_undertaken add constraint FK_PROJECT__PROJECT_U_PROJECT foreign key (jid)
      references project (jid) on delete restrict on update restrict;

alter table publish_paper add constraint FK_PUBLISH__PUBLISH_P_TEACHER foreign key (tid)
      references teacher (tid) on delete restrict on update restrict;

alter table publish_paper add constraint FK_PUBLISH__PUBLISH_P_PAPER foreign key (pid)
      references paper (pid) on delete restrict on update restrict;




Delimiter //
create procedure paper_del(IN pid INT, OUT state INT)
begin
   DECLARE s INT DEFAULT 0;
   DECLARE CONTINUE HANDLER FOR SQLEXCEPTION SET s = 1;
    START TRANSACTION;
    IF NOT EXISTS(select * from paper where paper.pid = pid) THEN
      SET s = 2;
   END IF;
    IF s = 0 THEN
      SET state = 0;
      DELETE from publish_paper WHERE publish_paper.pid = pid;
      DELETE from paper WHERE paper.pid = pid;
        commit;
   else
      SET state = s;
        rollback;
   end if;
END
//
