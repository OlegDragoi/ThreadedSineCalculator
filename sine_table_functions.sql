CREATE PROCEDURE clean()
BEGIN
	DECLARE max_divisor bigint unsigned;
	SELECT max(pi_divisor) into max_divisor FROM sine_values;
	
	IF max_divisor > 4 THEN
		DELETE FROM sine_values WHERE pi_divisor = max_divisor;
	END IF;
	
	SELECT max(pi_divisor) FROM sine_values;
END;

CREATE PROCEDURE get_initial_values()
BEGIN
	DECLARE max_divisor bigint unsigned;
	SELECT max(pi_divisor) into max_divisor FROM sine_values;
	SELECT x FROM sine_values WHERE pi_divisor = max_divisor;
END;