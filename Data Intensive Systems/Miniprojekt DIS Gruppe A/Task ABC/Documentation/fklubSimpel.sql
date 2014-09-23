CREATE TABLE date (
    dateid integer NOT NULL,
    year integer,
    month integer,
    day integer,
    special text
);

CREATE TABLE member (
    balance integer,
    year integer,
    memberid integer NOT NULL
);

CREATE TABLE product (
    productid integer NOT NULL,
    name text
);

CREATE TABLE sales (
    memberid integer,
    dateid integer,
    timeid integer,
    productid integer,
    sale integer,
    saleid integer NOT NULL
);

CREATE TABLE "time" (
    timeid integer NOT NULL,
    hour integer,
    minute integer
);

ALTER TABLE ONLY date
    ADD CONSTRAINT date_pkey PRIMARY KEY (dateid);

ALTER TABLE ONLY member
    ADD CONSTRAINT member_pkey PRIMARY KEY (memberid);

ALTER TABLE ONLY product
    ADD CONSTRAINT productid PRIMARY KEY (productid);

ALTER TABLE ONLY sales
    ADD CONSTRAINT sales_pkey PRIMARY KEY (saleid);

ALTER TABLE ONLY "time"
    ADD CONSTRAINT timeid PRIMARY KEY (timeid);

ALTER TABLE ONLY sales
    ADD CONSTRAINT dateid FOREIGN KEY (dateid) REFERENCES date(dateid);

ALTER TABLE ONLY sales
    ADD CONSTRAINT memberid FOREIGN KEY (memberid) REFERENCES member(memberid);

ALTER TABLE ONLY sales
    ADD CONSTRAINT producid FOREIGN KEY (productid) REFERENCES product(productid);

ALTER TABLE ONLY sales
    ADD CONSTRAINT timeid FOREIGN KEY (timeid) REFERENCES "time"(timeid);

