# Data Management Refactoring Implementation Plan

> **For agentic workers:** REQUIRED SUB-SKILL: Use superpowers:subagent-driven-development (recommended) or superpowers:executing-plans to implement this plan task-by-task. Steps use checkbox (`- [ ]`) syntax for tracking.

**Goal:** Refactor DataManager and its sub-managers (Item, District, Weather) for better initialization, caching, and singleton access.

**Architecture:** `DataManager` acts as a central hub (Singleton) that orchestrates the initialization of sub-managers. Sub-managers implement an `Initialize()` method for caching and setup.

**Tech Stack:** Unity (C#), ScriptableObjects, MonoBehaviours.

---

### Task 1: Refactor Sub-Managers (Initialize & Caching)

**Files:**
- Modify: `Assets/Scripts/Manager/ItemDataManager.cs`
- Modify: `Assets/Scripts/Data/DistrictDataManager.cs`
- Modify: `Assets/Scripts/Manager/WeatherDataManager.cs`

- [ ] **Step 1: Add Initialize() to ItemDataManager and implement Dictionary caching.**
- [ ] **Step 2: Ensure DistrictDataManager.Initialize() is properly implemented (already partially exists).**
- [ ] **Step 3: Add Initialize() to WeatherDataManager.**

### Task 2: High-Level DataManager Refactoring

**Files:**
- Modify: `Assets/Scripts/Manager/DataManager.cs`

- [ ] **Step 1: Update fields to use [SerializeField] and cleanup RequireComponent.**
- [ ] **Step 2: Implement Awake() to call Initialize() on all sub-managers.**
- [ ] **Step 3: Add Bridge methods (GetItem, GetDistrict, GetWeather).**

### Task 3: Safety & Verification

**Files:**
- Modify: `Assets/Scripts/Manager/DataManager.cs`

- [ ] **Step 1: Add [RequireComponent] where applicable.**
- [ ] **Step 2: Add Debug.LogError for missing references.**
- [ ] **Step 3: Commit and Push changes.**
