# ---------- TESTDATA ----------
insert into course value("c1", "数据库系统概论", 60, 1);
insert into course value("c2", "形式化方法", 40, 2);
insert into course value("c3", "人工智能基础", 60, 1);

insert into teacher value('t1', '张三', '1', '6');
insert into teacher value('t2', '张四', '2', '4');
insert into teacher value('t3', '张五', '2', '6');
insert into teacher value('t4', '张六', '1', '3');

insert into paper value(1, 'paper', 'ustc', '2009-01-16', 1, 1);
insert into paper value(2, 'paper', 'ustc', '2010-02-17', 1, 2);
insert into paper value(3, 'paper1', 'nju', '2011-03-18', 1, 3);
insert into paper value(4, 'paper2', 'nju', '2012-04-19', 1, 4);

insert into project value('j1', 'project', 'ustc', 1, 10000, 2009, 2009);
insert into project value('j2', 'project', 'ustc', 1, 5000, 2010, 2011);
insert into project value('j3', 'project1', 'nju', 1, 15000, 2011, 2012);

call course_taught_insert('t1', 'c3', 22, 1, 20, @s);
select @s where @s != 0;
call course_taught_insert('t2', 'c3', 22, 1, 40, @s);
select @s where @s != 0;
call course_taught_insert('t3', 'c1', 22, 2, 30, @s);
select @s where @s != 0;
call course_taught_insert('t4', 'c1', 22, 2, 30, @s);
select @s where @s != 0;
call course_taught_insert('t2', 'c2', 22, 1, 40, @s);
select @s where @s != 0;

call project_undertaken_insert('t1', 'j1', 1, 5000, @s);
select @s where @s != 0;
call project_undertaken_insert('t2', 'j1', 2, 5000, @s);
select @s where @s != 0;
call project_undertaken_insert('t3', 'j2', 1, 5000, @s);
select @s where @s != 0;
call project_undertaken_insert('t4', 'j3', 1, 10000, @s);
select @s where @s != 0;
call project_undertaken_insert('t1', 'j3', 2, 5000, @s);
select @s where @s != 0;

call publish_paper_insert('t1', 1, 1, true, @s);
select @s where @s != 0;
call publish_paper_insert('t1', 2, 1, true, @s);
select @s where @s != 0;
call publish_paper_insert('t1', 3, 1, true, @s);
select @s where @s != 0;
call publish_paper_insert('t2', 4, 1, true, @s);
select @s where @s != 0;
call publish_paper_insert('t3', 4, 2, false, @s);
select @s where @s != 0;
call publish_paper_insert('t4', 4, 3, false, @s);
select @s where @s != 0;
