CREATE TABLE employees (
    employee_id SERIAL PRIMARY KEY,
    first_name VARCHAR(50),
    last_name VARCHAR(50),
    position VARCHAR(50),
    salary NUMERIC(10, 2)
);

CREATE SCHEMA audit;

CREATE TABLE audit.changelog (
    change_id SERIAL PRIMARY KEY,
    table_name VARCHAR(50),
    operation_time TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    operation_type VARCHAR(10), -- Typ operacji: INSERT, UPDATE, DELETE
    record_id INT
);

CREATE TABLE audit.details (
    detail_id SERIAL PRIMARY KEY,
    change_id INT REFERENCES audit.changelog(change_id),
    column_name VARCHAR(50),
    old_value TEXT,
    new_value TEXT,
    username VARCHAR(50)
);

CREATE OR REPLACE FUNCTION audit_changes_insert() RETURNS TRIGGER AS $$
DECLARE
    ret_change_id INT;
BEGIN
    INSERT INTO audit.changelog (table_name, operation_type, record_id)
    VALUES (TG_TABLE_NAME, TG_OP, COALESCE(NEW.employee_id, OLD.employee_id))
    RETURNING change_id INTO ret_change_id;


    INSERT INTO audit.details (change_id, column_name, new_value, username)
    VALUES (ret_change_id, 'employee_id', NEW.employee_id, current_user);
    RETURN NEW;

END;
$$ LANGUAGE plpgsql;

CREATE OR REPLACE FUNCTION audit_changes_delete() RETURNS TRIGGER AS $$
DECLARE
    ret_change_id INT;
BEGIN

    INSERT INTO audit.changelog (table_name, operation_type, record_id)
    VALUES (TG_TABLE_NAME, TG_OP, COALESCE(NEW.employee_id, OLD.employee_id))
    RETURNING change_id INTO ret_change_id;

    INSERT INTO audit.details (change_id, column_name, old_value, username)
    VALUES (ret_change_id, 'employee_id', OLD.employee_id, current_user);
    RETURN NEW;
END;
$$ LANGUAGE plpgsql;

CREATE OR REPLACE FUNCTION audit_changes_update() RETURNS TRIGGER AS $$
DECLARE
    ret_change_id INT;
BEGIN
    INSERT INTO audit.changelog (table_name, operation_type, record_id)
    VALUES (TG_TABLE_NAME, TG_OP, COALESCE(NEW.employee_id, OLD.employee_id))
    RETURNING change_id INTO ret_change_id;

    IF NEW.first_name IS DISTINCT FROM OLD.first_name THEN
        INSERT INTO audit.details (change_id, column_name, old_value, new_value, username)
        VALUES (ret_change_id, 'first_name', OLD.first_name, NEW.first_name, current_user);
    END IF;
    IF NEW.last_name IS DISTINCT FROM OLD.last_name THEN
        INSERT INTO audit.details (change_id, column_name, old_value, new_value, username)
        VALUES (ret_change_id, 'last_name', OLD.last_name, NEW.last_name, current_user);
    END IF;
    IF NEW.position IS DISTINCT FROM OLD.position THEN
        INSERT INTO audit.details (change_id, column_name, old_value, new_value, username)
        VALUES (ret_change_id, 'position', OLD.position, NEW.position, current_user);
    END IF;
    IF NEW.salary IS DISTINCT FROM OLD.salary THEN
        raise notice 'jestem2';
        INSERT INTO audit.details (change_id, column_name, old_value, new_value, username)
        VALUES (ret_change_id, 'salary', OLD.salary::TEXT, NEW.salary::TEXT, current_user);
    END IF;
    RETURN NEW;
END;
$$ LANGUAGE plpgsql;

CREATE TRIGGER employees_insert_audit
AFTER INSERT ON employees
FOR EACH ROW
EXECUTE FUNCTION audit_changes_insert();

-- Trigger dla operacji UPDATE
CREATE TRIGGER employees_update_audit
AFTER UPDATE ON employees
FOR EACH ROW
EXECUTE FUNCTION audit_changes_update();

-- Trigger dla operacji DELETE
CREATE TRIGGER employees_delete_audit
AFTER DELETE ON employees
FOR EACH ROW
EXECUTE FUNCTION audit_changes_delete();

INSERT INTO employees (first_name, last_name, position, salary)
VALUES ('John', 'Doe', 'Developer', 60000);

-- UPDATE
UPDATE employees
SET salary = 65000
WHERE employee_id = 1;

-- DELETE
DELETE FROM employees
WHERE employee_id = 1;

select * from employees;
SELECT * FROM audit.changelog;

-- Sprawdzanie details
SELECT * FROM audit.details;