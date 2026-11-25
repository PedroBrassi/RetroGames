USE retrogames;

START TRANSACTION;

-- Teste 1. Plataforma inexistente. Esperado: erro 1452.
INSERT INTO tb_jogo
    (co_seq_jogo, no_jogo, nu_ano_lancamento, vl_preco, qt_estoque,
     dt_cadastro, co_plataforma, co_condicao_jogo)
VALUES
    (-901, 'Teste de chave estrangeira', 1990, 10.00, 1,
     UTC_TIMESTAMP(), -999, 2);

-- Teste 2. Nome de plataforma duplicado. Esperado: erro 1062.
INSERT INTO tb_plataforma
    (co_seq_plataforma, no_plataforma, no_fabricante, nu_ano_lancamento)
SELECT -902, no_plataforma, no_fabricante, nu_ano_lancamento
FROM tb_plataforma
WHERE co_seq_plataforma = 1;

-- Teste 3. Excluir plataforma com jogos. Esperado: erro 1451.
DELETE FROM tb_plataforma WHERE co_seq_plataforma = 1;

-- Teste 4. Nome de jogo nulo. Esperado: erro 1048.
INSERT INTO tb_jogo
    (co_seq_jogo, no_jogo, nu_ano_lancamento, vl_preco, qt_estoque,
     dt_cadastro, co_plataforma, co_condicao_jogo)
VALUES
    (-903, NULL, 1990, 10.00, 1, UTC_TIMESTAMP(), 1, 2);

-- Teste 5. Preço zero. Esperado: inserção aceita.
INSERT INTO tb_jogo
    (co_seq_jogo, no_jogo, nu_ano_lancamento, vl_preco, qt_estoque,
     dt_cadastro, co_plataforma, co_condicao_jogo)
VALUES
    (-904, 'Teste de preço zero', 1990, 0.00, 1, UTC_TIMESTAMP(), 1, 2);

SELECT co_seq_jogo, vl_preco FROM tb_jogo WHERE co_seq_jogo = -904;

ROLLBACK;

SELECT COUNT(*) AS jogos_de_teste_restantes
FROM tb_jogo
WHERE co_seq_jogo IN (-901, -903, -904);