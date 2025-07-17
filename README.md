
# 🚦 Infonetica – Configurable Workflow Engine (State Machine API)

A minimal, extensible backend service to:

- 🔧 Define custom workflows as state machines (states + actions)
- 🚀 Start workflow instances from definitions
- 🔁 Execute actions to transition between states with validation
- 📊 Inspect current state and history of any instance

> 🛠️ Built with ASP.NET Core (.NET 8 Minimal API)  
> 🎯 Submission for Infonetica Internship (Software Engineer)

---

## 🚀 Getting Started

### ✅ Prerequisites
- [.NET 8 SDK](https://dotnet.microsoft.com/en-us/download/dotnet/8.0)

### ▶️ Run the API
```bash
dotnet run
```

> Default port: `http://localhost:5165`

---

## 📘 API Endpoints

### 1️⃣ Create a Workflow Definition

`POST /definitions`

You can use the included `test.json` file as a sample definition.

**Example Body (`test.json`):**
```json
{
  "id": "leave-approval",
  "states": [
    { "id": "requested", "name": "Requested", "isInitial": true, "isFinal": false, "enabled": true },
    { "id": "approved", "name": "Approved", "isInitial": false, "isFinal": true, "enabled": true },
    { "id": "rejected", "name": "Rejected", "isInitial": false, "isFinal": true, "enabled": true }
  ],
  "actions": [
    { "id": "approve", "name": "Approve", "enabled": true, "fromStates": ["requested"], "toState": "approved" },
    { "id": "reject", "name": "Reject", "enabled": true, "fromStates": ["requested"], "toState": "rejected" }
  ]
}
```

---

### 2️⃣ Get Definition by ID

`GET /definitions/{id}`

---

### 3️⃣ List All Definitions

`GET /definitions`

---

### 4️⃣ Start Workflow Instance

`POST /instances?definitionId=leave-approval`

> Starts at the initial state defined in the workflow.

---

### 5️⃣ Execute an Action on Instance

`POST /instances/{instanceId}/execute?actionId=approve`

> Transitions to the target state if action is valid.

---

### 6️⃣ Get Instance State & History

`GET /instances/{instanceId}`

> Returns current state and list of executed actions with timestamps.

---

## 🧠 Assumptions & Design Notes

- In-memory storage only (no DB)
- Each definition must have **exactly one initial state**
- No transitions allowed from final states
- Actions only allowed if:
  - Action exists and is enabled
  - Current state is in `fromStates`
  - `toState` is enabled and exists
- Validation logic is **handled inside service classes** for clarity. `Validators/` folder is reserved for future modularization.

---

## 🧪 Testing with curl.exe (Windows)

```bash
curl.exe -X POST http://localhost:5165/definitions -H "Content-Type: application/json" -d "@test.json"

curl.exe -X POST "http://localhost:5165/instances?definitionId=leave-approval"

curl.exe -X POST "http://localhost:5165/instances/{instanceId}/execute?actionId=approve"

curl.exe http://localhost:5165/instances/{instanceId}
```

Replace `{instanceId}` with the one returned by the start-instance call.

---

## 📂 Project Structure

```
WorkflowEngine/
├── Models/                # Core data classes
├── Services/              # Business logic
├── Storage/               # In-memory repository
├── Validators/            # (Reserved for future use)
├── Program.cs             # Minimal API routing
├── README.md
```

---

## ✅ Submission

- 📁 Code pushed to public GitHub repo
- 📄 Includes all source files and this README
