# Unipay & Courts.Net Update Instructions

## Overview

This document outlines the changes made in two separate solutions and provides guidance on handling discrepancies between the available codebase and the deployed application.

---

## 🧾 Projects & Changes

### 🔹 Unipay Solution
- **Project**: `Unicomer.Cosacs.Repository`
- **Modified Class**: `PaymentRepository`

### 🔹 Courts.Net Solution
- **Project**: `Courts.NET.WS`
- **Modified File**: `WPaymentManager.asmx.cs`

---

## ⚠️ Important Note

There are known differences between the **available codebase** and the **deployed application**. To address and synchronize these discrepancies, follow the steps below.

---

## 🛠️ Steps to Retrieve and Modify Deployed Code

1. **Get the DLL**  
   - Access the deployed server and retrieve the required `.dll` file.

2. **Decompile the DLL**  
   - Use tools like **dnSpy** or similar to decompile the DLL into readable C# code.

3. **Edit the Code**  
   - Make the necessary changes in the decompiled code.

4. **Rebuild to Generate New DLL**  
   - Recompile the updated codebase to generate a new DLL, and deploy as required.

---

## 🔧 Tools Suggested
- [dnSpy](https://github.com/dnSpy/dnSpy)

---
