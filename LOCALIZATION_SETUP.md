# Настройка локализации в Watermelon Farmer

## Что было добавлено

1. **LocalizationManager.cs** - менеджер локализации (Singleton)
   - Автоматически определяет язык из `window.yandexLang` (WebGL)
   - По умолчанию: русский (ru)
   - Метод `Get(string key)` возвращает переведённую строку
   - Метод `GetFormatted(string key, params object[] args)` для форматирования строк с параметрами

2. **YandexBridge.jslib** - JavaScript bridge для получения языка
   - Расположен в `Assets/Plugins/`
   - Получает `window.yandexLang` и передаёт в Unity

3. **LocalizedText.cs** - компонент для TextMeshProUGUI
   - Добавляется на любой TMP текст
   - В Inspector указывается ключ локализации
   - Автоматически обновляет текст при старте

4. **Обновлён ShopUI.cs** - все тексты магазина теперь используют локализацию

## Как назначить ключи локализации

### Шаг 1: Добавить LocalizationManager на сцену

LocalizationManager автоматически создаётся в `MenuManager.Awake()`. Убедитесь, что на сцене MainMenu есть объект с `MenuManager` скриптом.

### Шаг 2: Добавить компонент LocalizedText на TMP текстовые элементы

Для каждого TextMeshProUGUI элемента в сценах **MainMenu** и **ArbuzFerma**:

1. Выберите текстовый объект в сцене
2. Добавьте компонент `LocalizedText` (Add Component → LocalizedText)
3. В поле `Localization Key` введите соответствующий ключ из списка ниже

### Шаг 3: Ключи локализации

#### MainMenu
| Ключ | RU | EN |
|------|-----|-----|
| `start_game` | НАЧАТЬ ИГРУ | START GAME |
| `settings` | НАСТРОЙКИ | SETTINGS |
| `quit` | ВЫЙТИ | QUIT |
| `empty_save` | ПУСТОЕ СОХРАНЕНИЕ | EMPTY SAVE |
| `play` | ИГРАТЬ | PLAY |
| `reset` | СБРОС | RESET |
| `rename` | ПЕРЕИМЕНОВАТЬ | RENAME |
| `back` | НАЗАД | BACK |

#### Shop
| Ключ | RU | EN |
|------|-----|-----|
| `open_shop` | ОТКРЫТЬ МАГАЗИН | OPEN SHOP |
| `upgrade_shop` | МАГАЗИН УЛУЧШЕНИЙ | UPGRADE SHOP |
| `grow_speed` | Скорость роста | Grow Speed |
| `harvest_value` | Ценность урожая | Harvest Value |
| `crit_harvest` | Крит. урожай | Crit Harvest |
| `fertilizer` | Удобрение | Fertilizer |
| `super_seeds` | Суперсемена | Super Seeds |
| `max_level` | МАКС УРОВЕНЬ | MAX LEVEL |
| `level` | УР. | LVL |
| `coins` | монет | coins |
| `unlock` | Разблокировать {0} | Unlock {0} |
| `all_unlocked` | ВСЕ СОРТА ОТКРЫТЫ | ALL VARIETIES UNLOCKED |

#### Game
| Ключ | RU | EN |
|------|-----|-----|
| `save` | СОХРАНИТЬ | SAVE |
| `exit` | ВЫЙТИ | EXIT |

#### Settings
| Ключ | RU | EN |
|------|-----|-----|
| `audio` | АУДИО | AUDIO |
| `music_volume` | Громкость музыки | Music Volume |
| `sfx_volume` | Громкость звуков | SFX Volume |

#### Shop Tooltips (динамические)
| Ключ | Формат (RU) | Формат (EN) |
|------|-------------|-------------|
| `tooltip_grow_speed` | Скорость роста (Ур. {0}/{1})\nТекущий: -{2:F1} сек → Следующий: -{3:F1} сек | Grow Speed (Lvl {0}/{1})\nCurrent: -{2:F1} sec → Next: -{3:F1} sec |
| `tooltip_grow_speed_max` | Скорость роста (МАКС УРОВЕНЬ) | Grow Speed (MAX LEVEL) |
| `tooltip_harvest_value` | Ценность урожая (Ур. {0}/{1})\nТекущий: +{2} монет → Следующий: +{3} монет | Harvest Value (Lvl {0}/{1})\nCurrent: +{2} coins → Next: +{3} coins |
| `tooltip_harvest_value_max` | Ценность урожая (МАКС УРОВЕНЬ) | Harvest Value (MAX LEVEL) |
| `tooltip_crit_chance` | Крит. шанс (Ур. {0}/{1})\nТекущий: {2}% → Следующий: {3}% | Crit Chance (Lvl {0}/{1})\nCurrent: {2}% → Next: {3}% |
| `tooltip_crit_chance_max` | Крит. шанс (МАКС УРОВЕНЬ) | Crit Chance (MAX LEVEL) |
| `tooltip_fertilizer` | Удобрение (Ур. {0}/{1})\nТекущий: +{2} монет → Следующий: +{3} монет | Fertilizer (Lvl {0}/{1})\nCurrent: +{2} coins → Next: +{3} coins |
| `tooltip_fertilizer_max` | Удобрение (МАКС УРОВЕНЬ) | Fertilizer (MAX LEVEL) |
| `tooltip_super_seed` | Суперсемена (Ур. {0}/{1})\nСтартовая стадия: {2} → {3} | Super Seeds (Lvl {0}/{1})\nStarting stage: {2} → {3} |
| `tooltip_super_seed_max` | Суперсемена (МАКС УРОВЕНЬ) | Super Seeds (MAX LEVEL) |
| `tooltip_watermelon_unlock` | Следующий сорт: {0}\nСтоимость: {1} монет\nДоход: +{2} монет/арбуз | Next variety: {0}\nCost: {1} coins\nIncome: +{2} coins/watermelon |
| `tooltip_all_unlocked` | Все сорта арбузов разблокированы! | All watermelon varieties unlocked! |
| `tooltip_max_upgrade` | Максимальный уровень улучшения! | Maximum upgrade level! |
| `tooltip_not_enough_coins` | Недостаточно монет! | Not enough coins! |

### Примеры использования

#### Простой текст (MainMenu):
```csharp
// В LocalizedText компоненте:
Localization Key: "start_game"
// При старте покажет "НАЧАТЬ ИГРУ" или "START GAME"
```

#### Форматированный текст (Shop):
```csharp
// Ключ: "grow_speed"
// Используется в ShopUI.cs:
LocalizationManager.Instance.GetFormatted("grow_speed", level, cost)
// Пример результата: "Скорость роста\nУР. 2: 100 монет"
```

### Шаг 4: Принудительное обновление всех текстов

Если нужно обновить все тексты при смене языка (в будущем), вызовите:
```csharp
LocalizedText.RefreshAllTexts();
```

## Важно

- Убедитесь, что все текстовые объекты имеют компонент **TextMeshProUGUI**
- Ключи должны точно совпадать с теми, что указаны в `LocalizationManager.InitializeTranslations()`
- В редакторе (не WebGL) язык всегда будет "ru"
- На WebGL язык определяется из `window.yandexLang` (передаётся из Яндекс.Игр)

## Файлы проекта

```
Assets/
├── Scripts/
│   ├── LocalizationManager.cs  (новый)
│   ├── LocalizedText.cs        (новый)
│   ├── Menu/
│   │   └── MenuManager.cs      (обновлён)
│   └── Ferma/
│       └── ShopUI.cs           (обновлён)
└── Plugins/
    └── YandexBridge.jslib      (новый)
```

## Тестирование

1. Запустите сцену MainMenu
2. Убедитесь, что LocalizationManager создаётся автоматически
3. Проверьте, что тексты с компонентом LocalizedText отображаются на русском
4. Для тестирования английского можно временно изменить в LocalizationManager:
   ```csharp
   currentLanguage = "en";