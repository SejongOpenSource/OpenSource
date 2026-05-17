# GEMINI.md - Convenience Store Management Game

This project is a Unity-based convenience store management simulation focusing on backend systems, data-driven simulations, and economic mechanics.

## ⚠️ Core Mandate: Role Limitation
- **Scope**: Focus EXCLUSIVELY on backend system logic, manager classes, and data structures.
- **Out of Scope**: 
    - UI implementation, visual object management (Prefabs/Sprites), and visual integration.
    - **Strictly Excluded**: `saleAlgorithm`, `CustomerManager`.
- **Task Focus**: Prioritize the task assigned in `HJMarkdownFile/focus.md`. Avoid speculative suggestions or unrequested "next steps".

## Project Overview
- **Architecture**: Singleton-based Managers (GameManager, TurnManager, SalesSimulationManager) with composition for sub-systems (Inventory, Loan, WeatherSystem, StoreManager).
- **Turn Phases**: Upgrade → Order → Simulation → Result.
- **Simulation Logic**: Calculates visitor counts and purchase probabilities based on District types (Business, Resident, Academy, Tourist, Campus) and Weather conditions.

## Development Workflow
### 3-File Queue System
Maintain context efficiency using files in `HJMarkdownFile/`:
1.  **focus.md**: The SINGLE active task. Read this at the start of every session.
2.  **pending.md**: Queue of future tasks/questions. Add here if new ideas arise mid-task.
3.  **resolved.md**: Knowledge base of past decisions and technical conclusions.

### Coding Principles (CLAUDE.md)
1.  **Think Before Coding**: State assumptions. Ask if unclear.
2.  **Simplicity First**: Minimum code to solve the problem. No speculative abstractions.
3.  **Surgical Changes**: Modify only what is necessary. Match existing style. No "adjacent cleanup".
4.  **Goal-Driven**: Define success criteria before implementation.

## Communication Style
- **Minimalist**: No prose around code blocks.
- **Terseness**: Use bullet points. No end-of-turn summaries.
- **One-Line Confirmations**: "Done." or specific file change status only.

## Key Technical Details
- **GameManager**: Central registry for all systems.
- **TurnManager**: Controls the game flow state machine (`TurnPhase`).
- **SalesSimulationManager**: Core business logic. Uses Coroutines for simulation steps.
- **Data Structures**: `DistrictData` and `ItemData` (ScriptableObjects) drive the simulation weights.

## Building and Testing
- **Unity Version**: Ensure compatibility with Unity 2022.3 LTS (assumed from URP assets).
- **Testing**: Use Unity Play Mode to verify logic changes in `SalesSimulationManager` and `TurnManager`.
- **Logic Validation**: Check `Debug.Log` outputs for visitor counts and sales calculations.
