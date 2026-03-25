# WebGL UI Fix Guide

## Описание проблемы
В WebGL версии игры возникали следующие проблемы с отображением UI:
1. Отсутствие заголовка "WATERMELON FARM"
2. UI смещен и масштабирован неправильно
3. По бокам появляются большие пустые области
4. Кнопки расположены некорректно относительно экрана

## Решение
Были внесены критические исправления для решения проблем:

### 1. WebGLCanvasScaler.cs (Исправлен)
**Назначение:** Оптимизация настроек CanvasScaler для WebGL
**Ключевые изменения:**
- Установлено matchWidthOrHeight = 0.5f (баланс между шириной и высотой)
- Используется referenceResolution 1920x1080
- Добавлены дополнительные настройки масштабирования (scaleFactor = 1f, dynamicPixelsPerUnit = 1f)
- Автоматически добавляется к Canvas в WebGL сборке через MenuManager

### 2. LocalizedText.cs (УДАЛЕН)
**Причина удаления:** Вызывал TMP OutOfRangeException в WebGL сборке
**Новое решение:** Локализация текста выполняется напрямую в MenuManager через публичные поля TextMeshProUGUI

### 3. BackgroundCanvasFixer.cs (Новый)
**Назначение:** Автоматический перенос фона в Canvas для правильного масштабирования
**Ключевые изменения:**
- Находит фоновый объект (по имени "Background" или тегу "Background")
- Перемещает его в Canvas и заменяет SpriteRenderer на Image
- Настраивает RectTransform для растягивания на весь экран
- Автоматически добавляется к Canvas в WebGL сборке

### 4. Упрощенная система локализации
**LocalizationManager.cs** (обновлен):
- Хранит переводы в двух словарях (русский и английский)
- Включен метод `GetFormatted` для форматированных строк с параметрами
- Все переводы встроены напрямую в код (проще поддержка)
- Поддержка динамического переключения языков

## Настройка в Unity Editor

### 1. Настройка Canvas
- Убедитесь, что Canvas имеет компонент CanvasScaler
- Render Mode: Screen Space - Overlay
- UI Scale Mode: Scale With Screen Size
- Reference Resolution: 1920 x 1080
- Screen Match Mode: Match Width Or Height
- Match: 0.5

### 2. Настройка фона
- Убедитесь, что фоновый объект называется "Background" или имеет тег "Background"
- При сборке для WebGL скрипт BackgroundCanvasFixer автоматически переместит его в Canvas
- Фон будет растягиваться на весь экран

### 3. Настройка заголовка "WATERMELON FARM"
- Найдите объект заголовка в сцене MainMenu.unity
- Убедитесь, что у него есть компонент TextMeshProUGUI
- В MenuManager найдите поле `titleText` и перетащите туда этот текстовый объект
- Настройте RectTransform:
  - Anchors: верхний центр (anchorMin = 0.5,1; anchorMax = 0.5,1)
  - Pivot: 0.5, 1
  - Position: Y = -100 (отступ от верха)
  - Размер: 800 x 100

### 4. Настройка кнопок (ВРУЧНУЮ в редакторе)
Для каждой кнопки (StartGameButton, SettingsButton, QuitButton):
- Убедитесь, что есть компонент Button и TextMeshProUGUI
- В MenuManager найдите соответствующие поля:
  - `startGameButtonText` → перетащите текстовый объект кнопки "Начать игру"
  - `settingsButtonText` → перетащите текстовый объект кнопки "Настройки"
  - `quitButtonText` → перетащите текстовый объект кнопки "Выход"
- Настройте RectTransform кнопок:
  - Anchors: центр (anchorMin = 0.5,0.5; anchorMax = 0.5,0.5)
  - Pivot: 0.5, 0.5
  - Position: распределите по вертикали (например, Y = 50, -10, -70)
  - Размер: 300 x 80

**Важно:** Автоматическая настройка кнопок удалена. Настройка делается вручную через привязку полей в MenuManager.

### 5. Привязка кнопок к функциям
В редакторе Unity найдите кнопки и привяжите их onClick события:
- StartGameButton → MenuManager.Instance.OpenSaveSlots()
- SettingsButton → MenuManager.Instance.OpenSettings()
- QuitButton → MenuManager.Instance.QuitGame()

## Локализация

### Изменение языка
```csharp
LocalizationManager.Instance.SetLanguage("ru"); // или "en"
```

### Получение перевода
```csharp
string text = LocalizationManager.Instance.Get("start_game");
```

### Форматированные строки
Для строк с параметрами используйте `GetFormatted`:
```csharp
string text = LocalizationManager.Instance.GetFormatted("tooltip_grow_speed", 
    currentLevel, maxLevel, effect, nextEffect);
// Результат: "Уровень 1/5\nЭффект: +50%\nСледующий: +100%"
```

### Добавление/изменение переводов в коде
```csharp
LocalizationManager.Instance.SetTranslation("new_key", "Русский текст", "English text");
```

## Панели меню
Убедитесь, что в MenuManager назначены:
- mainMenuPanel: главная панель меню
- saveSlotsPanel: панель выбора слотов
- settingsPanel: панель настроек

Панели должны быть расположены за пределами видимой области в начальном состоянии:
- saveSlotsPanel: anchoredPosition = (screenWidth, 0)
- settingsPanel: anchoredPosition = (-screenWidth, 0)

## Тестирование
1. Соберите проект для WebGL (File → Build Settings → WebGL → Build)
2. Откройте полученный HTML файл в браузере
3. Проверьте:
   - ✅ Заголовок "WATERMELON FARM" виден вверху по центру
   - ✅ Кнопки центрированы и равномерно распределены (настроены в редакторе)
   - ✅ Нет пустых областей по бокам
   - ✅ Фон растягивается на весь экран
   - ✅ UI корректно масштабируется при изменении размера окна
   - ✅ Кнопки "Начать игру", "Настройки", "Выход" работают
   - ✅ Магазин отображает форматированные тексты (уровни, эффекты, стоимость)

## Что было исправлено
1. **Заголовок**: Удалено скрытие заголовка в LocalizedText.Start()
2. **Масштабирование**: WebGLCanvasScaler теперь использует matchWidthOrHeight = 0.5f вместо 0f
3. **Пустые области**: Баланс между шириной и высотой устраняет боковые пустоты
4. **Фон**: Автоматический перенос фона в Canvas решает проблему с масштабированием
5. **Локализация**: Упрощенная система с встроенными переводами и методом GetFormatted
6. **TMP ошибка**: Удален LocalizedText.cs, локализация теперь выполняется напрямую в MenuManager

## Доступные ключи локализации
### Основные меню
- `game_title` - "WATERMELON FARM"
- `start_game` - "НАЧАТЬ ИГРУ"
- `settings` - "НАСТРОЙКИ"
- `quit` - "ВЫЙТИ"
- `save` - "СОХРАНИТЬ"
- `exit` - "ВЫХОД"
- `back` - "НАЗАД"
- `play` - "ИГРАТЬ"
- `reset` - "СБРОС"
- `rename` - "ПЕРЕИМЕНОВАТЬ"
- `empty_save` - "ПУСТОЕ СОХРАНЕНИЕ"
- `open_shop` - "МАГАЗИН"

### Настройки
- `audio` - "АУДИО"
- `music_volume` - "ГРОМКОСТЬ МУЗЫКИ"
- `sfx_volume` - "ГРОМКОСТЬ ЗВУКОВ"

### Интерфейс
- `slot` - "СЛОТ"
- `slot_number` - "Слот {0}"
- `save_name` - "Имя сохранения"
- `save_date` - "Дата"
- `new_save` - "НОВОЕ СОХРАНЕНИЕ"
- `load_save` - "ЗАГРУЗИТЬ"
- `delete_save` - "УДАЛИТЬ"

### Магазин
- `grow_speed` - "СКОРОСТЬ РОСТА"
- `harvest_value` - "СТОИМОСТЬ УРОЖАЯ"
- `crit_harvest` - "КРИТ. УРОЖАЙ"
- `fertilizer` - "УДОБРЕНИЕ"
- `super_seeds` - "СУПЕР СЕМЕНА"
- `level` - "УРОВЕНЬ"
- `coins` - "МОНЕТЫ"
- `max_level` - "МАКС. УРОВЕНЬ"
- `unlock` - "РАЗБЛОКИРОВАТЬ {0}"
- `all_unlocked` - "ВСЕ РАЗБЛОКИРОВАНО"

### Подсказки магазина (форматированные)
- `tooltip_grow_speed_max` - "Достигнут максимальный уровень"
- `tooltip_grow_speed` - "Уровень {0}/{1}\nЭффект: +{2}%\nСледующий: +{3}%"
- `tooltip_harvest_value_max` - "Достигнут максимальный уровень"
- `tooltip_harvest_value` - "Уровень {0}/{1}\nЭффект: +{2}%\nСледующий: +{3}%"
- `tooltip_crit_chance_max` - "Достигнут максимальный уровень"
- `tooltip_crit_chance` - "Уровень {0}/{1}\nЭффект: +{2}%\nСледующий: +{3}%"
- `tooltip_fertilizer_max` - "Достигнут максимальный уровень"
- `tooltip_fertilizer` - "Уровень {0}/{1}\nЭффект: +{2}%\nСледующий: +{3}%"
- `tooltip_super_seed_max` - "Достигнут максимальный уровень"
- `tooltip_super_seed` - "Уровень {0}/{1}\nЭффект: +{2}%\nСледующий: +{3}%"
- `tooltip_all_unlocked` - "Все арбузы разблокированы!"
- `tooltip_watermelon_unlock` - "Разблокировать {0}\nСтоимость: {1} монет\nХар-ка: {2}"
- `tooltip_max_upgrade` - "Достигнут максимальный уровень"
- `tooltip_not_enough_coins` - "Недостаточно монет"

## Важно
- Все изменения работают только в WebGL сборке (не в редакторе)
- Для полной функциональности необходимо настроить UI в редакторе Unity (вручную)
- **LocalizedText.cs удален** - больше не используется
- Локализация текста выполняется через MenuManager (поля titleText, startGameButtonText и т.д.)
- Переводы хранятся в коде LocalizationManager (простая редакция через код)
- При добавлении новых переводов обновляйте словари в LoadTranslations()
- WebGLUIFixer удален из-за конфликтов с TMP