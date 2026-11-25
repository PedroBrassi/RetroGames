USE retrogames;

START TRANSACTION;

INSERT INTO tb_condicao_jogo (co_condicao_jogo, ds_condicao_jogo)
VALUES (1, 'Novo'), (2, 'Usado'), (3, 'Recondicionado');

INSERT INTO tb_perfil (co_perfil, ds_perfil)
VALUES (1, 'Administrador'), (2, 'Cliente');

INSERT INTO tb_plataforma
    (co_seq_plataforma, no_plataforma, no_fabricante, nu_ano_lancamento)
VALUES
    (1, 'NES / Famicom', 'Nintendo', 1983),
    (2, 'Super Nintendo / Super Famicom', 'Nintendo', 1990),
    (3, 'Nintendo 64', 'Nintendo', 1996),
    (4, 'Mega Drive', 'Sega', 1988),
    (5, 'PlayStation', 'Sony', 1994),
    (6, 'Dreamcast', 'Sega', 1998);

INSERT INTO tb_genero (co_seq_genero, no_genero)
VALUES
    (1, 'Plataforma'),
    (2, 'Aventura'),
    (3, 'Ação'),
    (4, 'Corrida'),
    (5, 'RPG'),
    (6, 'Luta'),
    (7, 'Beat ''em up'),
    (8, 'Terror de sobrevivência');

INSERT INTO tb_jogo
    (co_seq_jogo, no_jogo, nu_ano_lancamento, vl_preco, qt_estoque,
     st_ativo, dt_cadastro, co_plataforma, co_condicao_jogo)
VALUES
    ( 1, 'Super Mario Bros.',          1985, 149.90, 4, TRUE,  UTC_TIMESTAMP(), 1, 2),
    ( 2, 'Super Mario Bros. 3',        1988, 189.90, 3, TRUE,  UTC_TIMESTAMP(), 1, 2),
    ( 3, 'The Legend of Zelda',        1986, 249.90, 2, TRUE,  UTC_TIMESTAMP(), 1, 3),
    ( 4, 'Metroid',                    1986, 219.90, 1, TRUE,  UTC_TIMESTAMP(), 1, 2),
    ( 5, 'Super Mario World',          1990, 199.90, 5, TRUE,  UTC_TIMESTAMP(), 2, 2),
    ( 6, 'Super Mario Kart',           1992, 229.90, 3, TRUE,  UTC_TIMESTAMP(), 2, 3),
    ( 7, 'Super Mario RPG',            1996, 349.90, 2, TRUE,  UTC_TIMESTAMP(), 2, 2),
    ( 8, 'Super Metroid',              1994, 289.90, 2, TRUE,  UTC_TIMESTAMP(), 2, 2),
    ( 9, 'Super Mario 64',             1996, 259.90, 4, TRUE,  UTC_TIMESTAMP(), 3, 2),
    (10, 'Mario Kart 64',              1996, 269.90, 3, TRUE,  UTC_TIMESTAMP(), 3, 3),
    (11, 'Paper Mario',                2000, 399.90, 1, TRUE,  UTC_TIMESTAMP(), 3, 2),
    (12, 'Zelda: Ocarina of Time',     1998, 329.90, 2, TRUE,  UTC_TIMESTAMP(), 3, 2),
    (13, 'Sonic the Hedgehog',         1991, 139.90, 4, TRUE,  UTC_TIMESTAMP(), 4, 2),
    (14, 'Sonic the Hedgehog 2',       1992, 159.90, 3, TRUE,  UTC_TIMESTAMP(), 4, 3),
    (15, 'Streets of Rage 2',          1992, 239.90, 2, TRUE,  UTC_TIMESTAMP(), 4, 2),
    (16, 'Golden Axe',                 1989, 149.90, 1, FALSE, UTC_TIMESTAMP(), 4, 2),
    (17, 'Crash Bandicoot',            1996, 179.90, 5, TRUE,  UTC_TIMESTAMP(), 5, 1),
    (18, 'Tekken 3',                   1998, 189.90, 0, TRUE,  UTC_TIMESTAMP(), 5, 2),
    (19, 'Resident Evil',             1996, 279.90, 2, TRUE,  UTC_TIMESTAMP(), 5, 2),
    (20, 'Final Fantasy VII',          1997, 299.90, 3, TRUE,  UTC_TIMESTAMP(), 5, 3);

INSERT INTO tb_jogo_genero (co_jogo, co_genero)
VALUES
    (1, 1), (2, 1), (3, 2), (3, 3), (4, 1), (4, 2), (4, 3),
    (5, 1), (6, 4), (7, 5), (8, 1), (8, 2), (8, 3),
    (9, 1), (10, 4), (11, 5), (12, 2), (12, 3),
    (13, 1), (14, 1), (15, 7), (16, 7),
    (17, 1), (18, 6), (19, 8), (20, 5);

COMMIT;