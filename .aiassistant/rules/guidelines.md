---
apply: always
---

# Game Development Conventions

Este documento consolida **CONVENTIONS.md**, o guia de estilo Unity C# e os princípios de arquitetura do projeto de jogo de cartas multiplayer.

Ele define **regras técnicas, arquiteturais e de estilo**, seguindo o mesmo espírito do `CONVENTIONS.md`: clareza, modularidade, testabilidade e escalabilidade.

---

## 1. Princípios Fundamentais

* O jogo é **determinístico** e **server-authoritative**
* Toda mutação de estado ocorre **exclusivamente no servidor**
* Clientes são apenas camada de **apresentação** (UI, VFX, animações)
* Nunca mutar `ScriptableObjects` em runtime
* Preferir clareza e explicitude a atalhos implícitos

---

## 2. Arquitetura Geral

### Decisões Arquiteturais-Chave

* **Event-Driven Core**: sistemas se comunicam apenas via eventos
* **Immutable State Pattern**: ações produzem novos estados
* **Server Authority**: lógica de jogo nunca roda no cliente
* **Command Pattern**: toda ação do jogador passa por validação
* **Dependency Injection**: injeção por construtor para testabilidade
* **Core puro em C#**: sem dependência de Unity no domínio

---

## 3. Arquitetura de Jogo de Cartas

* Cartas são **dados** (`ScriptableObject`)
* Efeitos são **objetos de lógica pura**
* Resolução usa **stack ou fila**, conforme o tipo de efeito
* Fases e passos são explícitos
* Regras são **event-driven**, nunca condicionais espalhados

---

## 4. Sistema de Efeitos

* Efeitos devem ser **atômicos, composáveis e reutilizáveis**
* Evitar efeitos monolíticos
* Tipos suportados:

    * Triggered Effects
    * Activated Effects
    * Continuous Effects
    * Replacement Effects (MTG-style)

Cada efeito deve declarar explicitamente:

* Timing
* Fonte
* Alvos
* Condições
* Autoridade (sempre servidor)

---

## 5. Networking (Netcode for GameObjects)

* `NetworkBehaviour` é usado apenas como **fronteira de sincronização**
* Lógica de jogo vive em serviços C# puros
* `ServerRpc`: intenção do jogador
* `ClientRpc`: feedback visual
* Nunca confiar em input do cliente
* Toda aleatoriedade é gerada no servidor

---

## 6. Separação de Camadas

Separação clara e obrigatória entre:

* Game State
* Rules Engine
* Networking Layer
* Presentation Layer

A lógica deve ser **testável fora do Unity**.

---

## 7. Independência de Sistemas

Cada sistema deve funcionar isoladamente e comunicar-se apenas via eventos:

* `TurnSystem`: controla turnos sem conhecer UI
* `ZoneSystem`: gerencia localizações de cartas
* `CardSystem`: lida com dados das cartas
* `RuleSystem`: valida ações sem executá-las

---

## 8. Organização de Código

### Ordem dentro das classes

1. Campos
2. Propriedades
3. Eventos
4. Métodos do MonoBehaviour
5. Métodos públicos
6. Métodos privados
7. Tipos auxiliares

Evite `#region`. Classes grandes indicam necessidade de refatoração.

---

## 9. Nomenclatura

* Legibilidade acima de brevidade
* Evitar abreviações
* Variáveis e tipos usam substantivos
* Métodos usam verbos

### Booleanos

* Devem formular perguntas
* Prefixos: `is`, `has`, `can`, `should`

---

## 10. Casing e Prefixos

| Elemento                  | Convenção   |
| ------------------------- | ----------- |
| Classes / Structs / Enums | PascalCase  |
| Métodos / Propriedades    | PascalCase  |
| Interfaces                | IPascalCase |
| Variáveis locais          | camelCase   |
| Campos privados           | m_ prefix   |
| Campos estáticos          | s_ prefix   |
| Constantes                | k_ prefix   |

---

## 11. Campos e Serialização (Unity)

* Nunca usar campos públicos para lógica
* Use `[SerializeField]` para exposição no Inspector
* Use `[Tooltip]` ao invés de comentários
* Evite inicializações redundantes

---

## 12. Propriedades e Métodos

* Prefira propriedades a campos públicos
* Métodos booleanos devem formular perguntas
* Evite métodos longos ou com muitos parâmetros
* Evite métodos com múltiplos modos via flags

---

## 13. Eventos

* Use `System.Action` quando possível
* Nomeie eventos como ações
* Diferencie eventos de antes e depois
* Métodos que disparam eventos usam prefixo `On`

---

## 14. Enums

### Enums simples

* Nome no singular
* PascalCase

### Enums com Flags

* Nome no plural
* Usar bitwise operations

---

## 15. Formatação

* Escolher K&R ou Allman e manter consistente
* Máx. 120 caracteres por linha
* Sempre usar chaves `{}`

---

## 16. Comentários e Documentação

* Comentários explicam o **porquê**, não o óbvio
* Prefira renomear código confuso
* APIs públicas devem ser documentadas
* Histórico pertence ao controle de versão

---

## 17. Namespaces

* Sempre usar namespaces
* PascalCase
* Refletir estrutura do projeto

---

## 18. Alertas Comuns

* Mutar ScriptableObjects em runtime
* Colocar lógica em MonoBehaviour
* Autoridade no cliente
* Uso incorreto de NetworkVariable
* Lógica não determinística
* Alocações excessivas de GC
* Uso de `Update()` para lógica baseada em turnos

---

## 19. Padrões Preferidos

* RulesEngine
* EffectResolver
* GameEventBus
* StackResolver
* ContinuousEffectLayer
* ReplacementEffect
* TriggerRegistry
* TurnPhaseController

---

## 20. Considerações Finais

Este documento segue o mesmo princípio do `CONVENTIONS.md`:

* Clareza > cleverness
* Consistência > preferência individual
* Arquitetura limpa reduz custo de manutenção

> Código deve ser escrito para humanos primeiro.
