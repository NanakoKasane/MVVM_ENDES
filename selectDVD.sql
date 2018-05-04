delimiter #
drop procedure if exists selectDVD #
create procedure selectDVD(codi smallint, out resul integer)
comment 'PA para pruebas desde .NET'

begin
	set @orden = 'select codigo, titulo, artista, pais, compania, precio, anio from dvd';
	
	if (codi is not null) then
		set @orden = concat(@orden, ' where codigo = ', codi);
	end if;
	
	
	-- Prepara la sentencia, la ejecuta y la libera de memoria. 
	PREPARE sentencia from @orden;
	EXECUTE sentencia;
	select FOUND_ROWS() into resul; -- LO MISMO QUE: set resul = (select FOUND_ROWS()); 
	DEALLOCATE PREPARE sentencia;
	
	-- Podemos coger con FOUND_ROWS() el número de filas encontradas (como un count). Y con ROW_COUNT() el número de filas afectadas (x Rows affected)
	
end #

delimiter ;