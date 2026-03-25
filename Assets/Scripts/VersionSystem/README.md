# Система истории версий (Patch Notes)

Полнофункциональная система для отображения истории версий игры.

## 📁 Структура файлов

```
Assets/Scripts/VersionSystem/
├── GameVersion.cs          - Структура данных версии
├── VersionManager.cs       - Менеджер версий (управление через скрипт)
├── VersionUIManager.cs     - Менеджер UI (управление панелью)
├── VersionPanelUI.cs       - Скрипт панели отображения версий
├── VersionNotifier.cs      - Уведомления об обновлениях (опционально)
├── VersionSystemTest.cs    - Тестовый скрипт для проверки
└── README.md              - Этот файл
```

## 🚀 Установка

### 1. VersionManager

1. Создайте GameObject `VersionManager` в сцене
2. Добавьте компонент `VersionManager`
3. В Inspector настройте:
   - `Current Version` - актуальная версия игры (например, "0.1.3")
   - `Versions Text File` - перетащите текстовый файл с версиями (см. формат ниже)
   - `Load From File On Start` = true (загружать при старте)
   - `Current Version Text` (опционально) - TextMeshProUGUI для отображения версии

### 2. VersionPanel (панель версий)

1. В Canvas создайте Panel → назовите `VersionPanel`
2. Добавьте компонент `VersionPanelUI`
3. Настройте RectTransform (Anchor: Center, Size: 600x500)
4. Добавьте Image для фона (цвет по желанию)
5. Внутри Panel создайте ScrollView:
   - Viewport с Mask
   - Content (Vertical Layout Group)
   - В Content добавьте TextMeshPro Text
6. Назначьте этот Text в поле `Version Text` компонента VersionPanelUI

### 3. Кнопка открытия

1. Создайте Button в Canvas
2. В инспекторе кнопки → OnClick()
3. Добавьте объект с `VersionUIManager`
4. Выберите: `VersionUIManager → ShowVersionPanel()`

### 4. VersionUIManager

1. Создайте GameObject `VersionUIManager`
2. Добавьте компонент `VersionUIManager`

### 5. VersionNotifier (опционально)

1. Создайте GameObject `VersionNotifier`
2. Добавьте компонент `VersionNotifier`
3. Настройте `Notification Duration`

### 6. Интеграция с MenuManager

В MenuManager добавьте кнопку "Версии" и настройте OnClick() → `MenuManager → OpenVersions()`

## 📝 Формат текстового файла версий

Создайте текстовый файл (например, `versions.txt`) и перетащите его в поле `Versions Text File`:

```
версия: 0.1.0
изменения: - Добавлены арбузы
- Добавлены улучшения
- Базовая экономика
---
версия: 0.1.1
изменения: - Исправлен баланс
- Добавлена бесконечная прокачка
---
версия: 0.1.2
изменения: - Добавлена система локализации
- Русский и английский языки
- Улучшен UI
```

**Формат:**
- `версия:` или `version:` - номер версии
- `изменения:` или `changes:` - список изменений (переносы строк через реальные \n)
- `---` - разделитель версий (необязательно)

## 🔧 Управление версиями через скрипт

```csharp
// Добавить версию
VersionManager.Instance.AddVersion("0.1.4", "- Новый арбуз\n- Исправление багов");

// Удалить версию
VersionManager.Instance.RemoveVersion("0.1.0");

// Очистить все версии
VersionManager.Instance.ClearVersions();

// Изменить текущую версию
VersionManager.Instance.SetCurrentVersion("0.2.0");

// Получить список версий
List<GameVersion> allVersions = VersionManager.Instance.GetVersions();

// Подписаться на изменения
VersionManager.Instance.OnVersionChanged += () => {
    Debug.Log("Версии обновлены!");
};
```

## 🎯 Как это работает

1. **VersionManager** загружает версии из текстового файла при старте
2. **VersionPanelUI** отображает все версии в обратном порядке (новые сверху)
3. **VersionUIManager** управляет открытием/закрытием панели
4. **VersionNotifier** проверяет при старте, новая ли версия
5. Версия сохраняется в PlayerPrefs после первого показа

## 🎨 Настройка

### Отображение текущей версии
Привяжите TextMeshProUGUI в поле `Current Version Text` в VersionManager. Текст автоматически обновится.

### Анимации
В `VersionPanelUI` настройте:
- `Open Duration` / `Close Duration`
- `Open Ease` / `Close Ease`

### Локализация
Добавьте ключи в LocalizationManager:
- `versions` - текст кнопки
- `update_available` - заголовок уведомления
- `update_message` - сообщение уведомления
- `view_changes` - кнопка "Посмотреть"

## 🧪 Тестирование

Используйте `VersionSystemTest`:
- `TestShowVersionPanel()` - открыть панель
- `TestCheckVersion()` - проверить версию
- `TestResetSavedVersion()` - сбросить сохраненную версию

## 📋 Требования к UI

**Панель VersionPanel:**
- Компонент `VersionPanelUI`
- TextMeshProUGUI (назначить в `Version Text`)
- ScrollView для прокрутки

**Кнопка:**
- Button → OnClick() → `VersionUIManager.ShowVersionPanel()`

## 💡 Особенности

- ✅ Управление версиями через скрипт (AddVersion, RemoveVersion, ClearVersions)
- ✅ Загрузка из текстового файла (TextAsset)
- ✅ Привязка TextMeshProUGUI для отображения текущей версии
- ✅ Прокрутка через ScrollView
- ✅ Анимации открытия/закрытия
- ✅ Событие OnVersionChanged
- ✅ Метод SetCurrentVersion() для изменения версии

---

**Система готова к использованию!**