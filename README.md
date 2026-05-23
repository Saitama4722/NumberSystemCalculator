# 🧮 Number System Calculator

[![.NET Framework](https://img.shields.io/badge/.NET%20Framework-4.7.2-512BD4?style=flat-square&logo=dotnet)](https://dotnet.microsoft.com/)
[![Language](https://img.shields.io/badge/Language-C%23-239120?style=flat-square&logo=csharp)](https://learn.microsoft.com/en-us/dotnet/csharp/)
[![Platform](https://img.shields.io/badge/Platform-Windows-0078D6?style=flat-square&logo=windows)](https://www.microsoft.com/windows)
[![License](https://img.shields.io/badge/License-MIT-yellow?style=flat-square)](LICENSE)
[![UI](https://img.shields.io/badge/UI-WinForms-blue?style=flat-square)](https://learn.microsoft.com/en-us/dotnet/desktop/winforms/)

A modern, dark-themed Windows Forms calculator that works with **four number systems simultaneously** — Binary, Octal, Decimal and Hexadecimal. Every input is converted live across all four bases, with full support for arithmetic and bitwise operations and a built-in history panel.

![Screenshot](screenshot.png)

---

## ✨ Features

- 🔢 **Four number systems**: Binary (BIN), Octal (OCT), Decimal (DEC), Hexadecimal (HEX)
- ➕ **Arithmetic operations**: addition (`+`), subtraction (`-`), multiplication (`*`), integer division (`/`)
- 🔬 **Bitwise operations**: `AND`, `OR`, `XOR`, `NOT`, left shift (`<<`), right shift (`>>`)
- 🔄 **Real-time conversion**: enter a number in any base and watch the other three update instantly
- 🛡️ **Smart input validation**: only digits valid for the active base are accepted; A–F are auto-disabled outside HEX
- 📜 **Calculation history**: last 15 operations shown in a scrollable side panel, with a clear-history action
- ⌨️ **Full keyboard support**: digits, operators, `Enter` for `=`, `Esc` to clear, `Backspace` to delete
- 🌙 **Modern dark UI**: charcoal background `#1E1E2E`, purple/blue accents, flat buttons with hover & press states
- ⚠️ **Robust error handling**: division by zero, overflow, and invalid input are reported in the status bar

---

## 🛠️ Technologies

| Component | Choice |
|---|---|
| Language | **C#** |
| Framework | **.NET Framework 4.7.2** |
| UI toolkit | **Windows Forms (WinForms)** |
| IDE | **Visual Studio 2019** or newer |
| Architecture | Separation between conversion (`NumberConverter`), state (`CalculatorEngine`) and UI (`Form1`) |
| Algorithms | Manual conversion — **Horner's scheme** for parsing, **repeated division** for formatting (no blind `Convert.ToInt64`) |

---

## 📁 Project Structure

```
NumberSystemCalculator/
├── NumberSystemCalculator.sln           ← Visual Studio solution
├── README.md
├── LICENSE
└── NumberSystemCalculator/
    ├── NumberSystemCalculator.csproj
    ├── Program.cs                       ← Application entry point
    ├── Form1.cs                         ← Main window and event handlers
    ├── Form1.Designer.cs                ← Auto-generated designer code
    ├── Form1.resx
    ├── NumberConverter.cs               ← Static conversion utilities
    ├── CalculatorEngine.cs              ← State, history, operations
    └── Properties/
        └── AssemblyInfo.cs
```

---

## 🚀 How to Run

```bash
# 1. Clone the repository
git clone https://github.com/Saitama4722/NumberSystemCalculator.git

# 2. Open the solution
cd NumberSystemCalculator
start NumberSystemCalculator.sln
```

3. In Visual Studio press **F5** to run with debugging, or **Ctrl + F5** for run-without-debugging.

The compiled binary will appear at `NumberSystemCalculator/bin/Debug/NumberSystemCalculator.exe`.

---

## 💻 System Requirements

- **OS**: Windows 7 / 8 / 10 / 11
- **.NET Framework**: 4.7.2 or higher
- **IDE**: Visual Studio 2019 or newer (Community Edition is enough)
- **Display**: minimum resolution 1024 × 768

---

## ⌨️ Keyboard Shortcuts

| Key | Action |
|---|---|
| `0`–`9`, `A`–`F` | Enter a digit (when valid for the active base) |
| `Enter` | Equals (`=`) — finalise the current calculation |
| `Esc` | Clear (C) — reset accumulator and display |
| `Backspace` | Delete the last entered digit |
| `+` `-` `*` `/` | Arithmetic operations |
| `&` | Bitwise **AND** |
| `\|` | Bitwise **OR** |
| `^` | Bitwise **XOR** |
| `~` | Bitwise **NOT** (unary) |

---

<br>
<br>

---

# 🧮 Калькулятор систем счисления

[![.NET Framework](https://img.shields.io/badge/.NET%20Framework-4.7.2-512BD4?style=flat-square&logo=dotnet)](https://dotnet.microsoft.com/)
[![Язык](https://img.shields.io/badge/Язык-C%23-239120?style=flat-square&logo=csharp)](https://learn.microsoft.com/ru-ru/dotnet/csharp/)
[![Платформа](https://img.shields.io/badge/Платформа-Windows-0078D6?style=flat-square&logo=windows)](https://www.microsoft.com/windows)
[![Лицензия](https://img.shields.io/badge/Лицензия-MIT-yellow?style=flat-square)](LICENSE)
[![UI](https://img.shields.io/badge/UI-WinForms-blue?style=flat-square)](https://learn.microsoft.com/ru-ru/dotnet/desktop/winforms/)

Современный калькулятор с тёмной темой на Windows Forms, который одновременно работает в **четырёх системах счисления** — двоичной, восьмеричной, десятичной и шестнадцатеричной. Любое введённое число мгновенно отображается во всех четырёх основаниях; поддерживаются арифметические и побитовые операции, а также панель истории вычислений.

![Скриншот](screenshot.png)

---

## ✨ Возможности

- 🔢 **Четыре системы счисления**: двоичная (BIN), восьмеричная (OCT), десятичная (DEC), шестнадцатеричная (HEX)
- ➕ **Арифметические операции**: сложение (`+`), вычитание (`-`), умножение (`*`), целочисленное деление (`/`)
- 🔬 **Побитовые операции**: `AND`, `OR`, `XOR`, `NOT`, сдвиг влево (`<<`), сдвиг вправо (`>>`)
- 🔄 **Конвертация в реальном времени**: введите число в любой из систем — три оставшиеся обновятся мгновенно
- 🛡️ **Умная проверка ввода**: принимаются только цифры, допустимые для активной системы; кнопки A–F автоматически блокируются вне HEX
- 📜 **История вычислений**: последние 15 операций в прокручиваемой панели справа, с кнопкой очистки
- ⌨️ **Полная поддержка клавиатуры**: цифры, операторы, `Enter` для `=`, `Esc` для сброса, `Backspace` для удаления
- 🌙 **Современный тёмный интерфейс**: фон `#1E1E2E`, фиолетово-синие акценты, плоские кнопки с анимациями наведения и нажатия
- ⚠️ **Надёжная обработка ошибок**: деление на ноль, переполнение и недопустимый ввод выводятся в строку состояния

---

## 🛠️ Технологии

| Компонент | Выбор |
|---|---|
| Язык | **C#** |
| Платформа | **.NET Framework 4.7.2** |
| UI-фреймворк | **Windows Forms (WinForms)** |
| Среда разработки | **Visual Studio 2019** или новее |
| Архитектура | Разделение: конвертация (`NumberConverter`), состояние (`CalculatorEngine`), интерфейс (`Form1`) |
| Алгоритмы | Ручная реализация — **схема Горнера** для разбора и **повторное деление** для формирования (без слепого `Convert.ToInt64`) |

---

## 📁 Структура проекта

```
NumberSystemCalculator/
├── NumberSystemCalculator.sln           ← Решение Visual Studio
├── README.md
├── LICENSE
└── NumberSystemCalculator/
    ├── NumberSystemCalculator.csproj
    ├── Program.cs                       ← Точка входа в приложение
    ├── Form1.cs                         ← Главное окно и обработчики событий
    ├── Form1.Designer.cs                ← Автогенерируемый код дизайнера
    ├── Form1.resx
    ├── NumberConverter.cs               ← Статические утилиты конвертации
    ├── CalculatorEngine.cs              ← Состояние, история, операции
    └── Properties/
        └── AssemblyInfo.cs
```

---

## 🚀 Как запустить

```bash
# 1. Клонируйте репозиторий
git clone https://github.com/Saitama4722/NumberSystemCalculator.git

# 2. Откройте решение
cd NumberSystemCalculator
start NumberSystemCalculator.sln
```

3. В Visual Studio нажмите **F5** для запуска с отладкой или **Ctrl + F5** для запуска без отладки.

Скомпилированный исполняемый файл появится в `NumberSystemCalculator/bin/Debug/NumberSystemCalculator.exe`.

---

## 💻 Системные требования

- **ОС**: Windows 7 / 8 / 10 / 11
- **.NET Framework**: 4.7.2 или выше
- **Среда разработки**: Visual Studio 2019 или новее (достаточно Community Edition)
- **Экран**: минимальное разрешение 1024 × 768

---

## ⌨️ Горячие клавиши

| Клавиша | Действие |
|---|---|
| `0`–`9`, `A`–`F` | Ввод цифры (если она допустима в активной системе) |
| `Enter` | Равно (`=`) — завершить вычисление |
| `Esc` | Сброс (C) — очистить аккумулятор и дисплей |
| `Backspace` | Удалить последнюю введённую цифру |
| `+` `-` `*` `/` | Арифметические операции |
| `&` | Побитовое **AND** (И) |
| `\|` | Побитовое **OR** (ИЛИ) |
| `^` | Побитовое **XOR** (исключающее ИЛИ) |
| `~` | Побитовое **NOT** (унарное НЕ) |
