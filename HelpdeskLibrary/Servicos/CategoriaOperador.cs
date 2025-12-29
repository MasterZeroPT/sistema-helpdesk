/**
 * @author Pedro Andrade
 * @email a31497@alunos.ipca.pt
 * @create date 2025-11 
 * @desc Classe estática para categorizar operadores com base na sua especialidade
 */
using Callcenter.HelpdeskLibrary.Entidades;
using Callcenter.HelpdeskLibrary.Enums;
using System;

namespace Callcenter.HelpdeskLibrary.Servicos
{
    /// <summary>
    /// Classe estática (utility class) responsável por:
    /// 1. Classificar operadores automaticamente baseado em especialidade
    /// 2. Promover/despromover operadores entre níveis de acesso
    /// 
    /// Padrões de design:
    /// - Métodos sobrecarregados (overloading): mesma função, parâmetros diferentes
    /// - Static class: não pode ser instanciada, apenas métodos utilitários
    /// </summary>
    public static class CategoriaOperador
    {
        /// <summary>
        /// Classifica um operador baseado na sua especialidade.
        /// Sobrecarga (overload) do método Classificar() que aceita objeto Operador.
        /// </summary>
        /// <param name="operador">Operador a ser classificado</param>
        /// <returns>Nível de acesso apropriado para a especialidade</returns>
        public static NivelAcesso Classificar(Operador operador)
        {
            // Delega para a versão que aceita string
            return Classificar(operador.Especialidade);
        }

        /// <summary>
        /// Classifica baseado na string de especialidade.
        /// Usa switch expression (C# 8+) para mapeamento limpo.
        /// Pattern matching: compara string e retorna NivelAcesso correspondente.
        /// </summary>
        /// <param name="especialidade">Nome da especialidade (pode ser null)</param>
        /// <returns>NivelAcesso apropriado (default = Nivel1 se não reconhecido)</returns>
        public static NivelAcesso Classificar(string? especialidade)
        {
            return especialidade switch
            {
                "Técnico" => NivelAcesso.Nivel1,   // Operadores técnicos de base
                "Suporte" => NivelAcesso.Nivel2,   // Suporte especializado
                "Gestão" => NivelAcesso.Nivel3,    // Gestores/supervisores
                _ => NivelAcesso.Nivel1            // Default: qualquer outra especialidade ou null
            };
        }

        /// <summary>
        /// Aplica classificação automática a um operador existente.
        /// Atualiza diretamente a propriedade Nivel do operador.
        /// </summary>
        /// <param name="operador">Operador a ter nível atualizado</param>
        /// <returns>true sempre (indica sucesso da operação)</returns>
        public static bool AplicarClassificacao(Operador operador)
        {
            operador.Nivel = Classificar(operador.Especialidade);
            return true;
        }

        /// <summary>
        /// Promove um nível de acesso para o próximo nível superior.
        /// Versão que trabalha diretamente com o enum NivelAcesso (sem operador).
        /// Usa switch expression com pattern matching.
        /// </summary>
        /// <param name="nivel">Nível atual</param>
        /// <returns>Próximo nível (ou mesmo nível se já no máximo)</returns>
        public static NivelAcesso Promover(NivelAcesso nivel)
        {
            return nivel switch
            {
                NivelAcesso.Nivel1 => NivelAcesso.Nivel2,  // Nivel1 -> Nivel2
                NivelAcesso.Nivel2 => NivelAcesso.Nivel3,  // Nivel2 -> Nivel3
                _ => nivel  // Nivel3 ou outro: mantém inalterado (já no máximo)
            };
        }

        /// <summary>
        /// Despromove um nível de acesso para o nível inferior.
        /// Versão que trabalha diretamente com o enum NivelAcesso.
        /// </summary>
        /// <param name="nivel">Nível atual</param>
        /// <returns>Nível anterior (ou mesmo nível se já no mínimo)</returns>
        public static NivelAcesso Despromover(NivelAcesso nivel)
        {
            return nivel switch
            {
                NivelAcesso.Nivel3 => NivelAcesso.Nivel2,  // Nivel3 -> Nivel2
                NivelAcesso.Nivel2 => NivelAcesso.Nivel1,  // Nivel2 -> Nivel1
                _ => nivel  // Nivel1 ou outro: mantém inalterado (já no mínimo)
            };
        }

        /// <summary>
        /// Promove um operador para o próximo nível de acesso.
        /// Versão que trabalha com objeto Operador e faz validações.
        /// Lança exceção se já estiver no nível máximo (regra de negócio).
        /// </summary>
        /// <param name="operador">Operador a ser promovido</param>
        /// <returns>true se promovido com sucesso</returns>
        /// <exception cref="InvalidOperationException">Se já estiver no Nivel3</exception>
        public static bool Promover(Operador operador)
        {
            // Validação: não pode promover se já está no topo
            if (operador.Nivel == NivelAcesso.Nivel3) 
            { 
                throw new InvalidOperationException($"O operador {operador.Nome} já está no nível máximo.");
            }
            // Aplica promoção usando a versão que trabalha com enum
            operador.Nivel = Promover(operador.Nivel);
            return true;
        }

        /// <summary>
        /// Despromove um operador para o nível de acesso anterior.
        /// Lança exceção se já estiver no nível mínimo (regra de negócio).
        /// </summary>
        /// <param name="operador">Operador a ser despromovido</param>
        /// <returns>true se despromovido com sucesso</returns>
        /// <exception cref="InvalidOperationException">Se já estiver no Nivel1</exception>
        public static bool Despromover(Operador operador)
        {
            // Validação: não pode despromover se já está no mínimo
            if (operador.Nivel == NivelAcesso.Nivel1)
            {
                throw new InvalidOperationException($"O operador {operador.Nome} já está no nível mínimo (Nivel1) e não pode ser despromovido.");
            }
            // Aplica despromoção usando a versão que trabalha com enum
            operador.Nivel = Despromover(operador.Nivel);
            return true;
        }
    }
}