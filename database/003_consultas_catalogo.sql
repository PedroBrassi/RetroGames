USE retrogames;

-- 1. Catálogo de jogos ativos.
SELECT j.no_jogo, p.no_plataforma, c.ds_condicao_jogo, j.vl_preco
FROM tb_jogo AS j
JOIN tb_plataforma AS p ON p.co_seq_plataforma = j.co_plataforma
JOIN tb_condicao_jogo AS c ON c.co_condicao_jogo = j.co_condicao_jogo
WHERE j.st_ativo = TRUE
ORDER BY j.no_jogo, j.co_seq_jogo;

-- 2. Gêneros de um jogo. ID 4 = Metroid.
SET @id_jogo = 4;

SELECT g.no_genero
FROM tb_jogo_genero AS jg
JOIN tb_genero AS g ON g.co_seq_genero = jg.co_genero
WHERE jg.co_jogo = @id_jogo
ORDER BY g.no_genero;

-- 3. Jogos de um gênero. ID 4 = Corrida.
SET @id_genero = 4;

SELECT j.no_jogo
FROM tb_jogo_genero AS jg
JOIN tb_jogo AS j ON j.co_seq_jogo = jg.co_jogo
WHERE jg.co_genero = @id_genero
ORDER BY j.no_jogo, j.co_seq_jogo;

-- 4. Quantidade de jogos por plataforma, incluindo plataformas vazias.
SELECT p.no_plataforma, COUNT(j.co_seq_jogo) AS total_jogos
FROM tb_plataforma AS p
LEFT JOIN tb_jogo AS j ON j.co_plataforma = p.co_seq_plataforma
GROUP BY p.co_seq_plataforma, p.no_plataforma
ORDER BY p.no_plataforma;

-- 5. Busca por parte do nome, com cinco itens por página.
SET @busca = 'Mario';

SELECT no_jogo
FROM tb_jogo
WHERE st_ativo = TRUE AND no_jogo LIKE CONCAT('%', @busca, '%')
ORDER BY no_jogo, co_seq_jogo
LIMIT 5 OFFSET 0;

SELECT no_jogo
FROM tb_jogo
WHERE st_ativo = TRUE AND no_jogo LIKE CONCAT('%', @busca, '%')
ORDER BY no_jogo, co_seq_jogo
LIMIT 5 OFFSET 5;