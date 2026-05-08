# Git Commit Messages

## Базовый формат

Сообщения коммитов должны соответствовать структуре Conventional Commits, но начинаться с соответствующего emoji.

Формат первой строки:

`:emoji: type(scope)!: description`

Где:

- `:emoji:` — emoji-код из `.agents/GitCommitEmoji.md`.
- `type` — тип изменения.
- `scope` — необязательная область изменения.
- `!` — признак breaking change, используется только при несовместимых изменениях.
- `description` — краткое описание изменения на английском языке.

## Примеры

```text
:sparkles: feat(api): add product shipment notification
:bug: fix(auth): prevent token refresh race condition
:books: docs(readme): add local setup instructions
:hammer: refactor(core): simplify routing strategy selection
:rotating_light: test(api): cover product shipment notification
:green_heart: ci(build): add Windows smoke test
:whale: chore(docker): update local development compose file
:boom: feat(config)!: replace legacy configuration format
```

## Разрешенные type

Используй стандартные типы Conventional Commits:

- `feat` — новая функциональность.
- `fix` — исправление ошибки.
- `docs` — документация.
- `style` — изменения форматирования, CSS, внешнего вида без изменения логики.
- `refactor` — рефакторинг без изменения поведения.
- `perf` — улучшение производительности.
- `test` — тесты.
- `build` — сборка, зависимости, packaging.
- `ci` — CI/CD.
- `chore` — технические изменения без изменения бизнес-логики.
- `revert` — откат изменений.

Допустимые дополнительные типы, если они точнее описывают изменение:

- `security` — изменения безопасности.
- `i18n` — локализация и переводы.
- `a11y` — доступность.
- `config` — конфигурация.

## Emoji

Emoji выбирается по смыслу изменения из файла:

`.agents/GitCommitEmoji.md`

Если изменение подходит под несколько категорий, выбирай emoji по главной причине коммита.

Рекомендуемые соответствия:

```text
:tada: initial commit
:bookmark: version tag
:sparkles: feat
:bug: fix
:books: docs
:bulb: docs for source code
:racehorse: perf
:lipstick: style / cosmetic UI
:rotating_light: test
:white_check_mark: add test
:heavy_check_mark: make test pass
:zap: general update
:art: improve structure / formatting
:hammer: refactor
:fire: remove code / files
:green_heart: ci
:lock: security
:arrow_up: upgrade dependencies
:arrow_down: downgrade dependencies
:shirt: lint
:alien: translation / external API change
:pencil: text
:ambulance: critical hotfix
:rocket: deploy
:apple: macOS fix
:penguin: Linux fix
:checkered_flag: Windows fix
:construction: work in progress
:construction_worker: CI build system
:chart_with_upwards_trend: analytics
:heavy_minus_sign: remove dependency
:heavy_plus_sign: add dependency
:whale: docker
:wrench: configuration
:package: package files
:twisted_rightwards_arrows: merge
:rewind: revert
:boom: breaking change
:ok_hand: code review changes
:wheelchair: accessibility
:truck: move / rename
```

## Правила description

Description должен быть:

- на английском языке;
- в нижнем регистре, кроме имен собственных;
- в imperative mood;
- без точки в конце;
- не длиннее 72 символов, если возможно;
- конкретным, без общих формулировок вроде `update files`, `fix bugs`, `changes`.

Хорошо:

```text
:sparkles: feat(search): add product results page
:bug: fix(layout): prevent code block scrollbar overlap
:books: docs(dev): document clean Bitrix reset flow
```

Плохо:

```text
:sparkles: feat: update
:bug: fix: bugs
:books: docs: changes
```

## Scope

Scope должен быть коротким и технически точным.

Примеры scope:

```text
api
ui
auth
catalog
search
docker
ci
docs
config
admin
layout
```

Если область изменения неочевидна или изменение затрагивает много частей, scope можно не указывать.

Допустимо:

```text
:zap: chore: update project metadata
```

## Breaking changes

Если изменение ломает совместимость, добавляй `!` после `type` или `scope`.

Пример:

```text
:boom: feat(config)!: replace legacy environment variables
```

Для breaking change желательно добавить footer:

```text
BREAKING CHANGE: legacy environment variables are no longer supported.
```

## Body

Body используется только если первая строка недостаточна.

Формат:

```text
:emoji: type(scope): description

Explain what changed and why.

Mention important constraints, migration notes, or testing details.
```

## Footer

Footer используется для:

- `BREAKING CHANGE`;
- ссылок на issue;
- ссылок на task;
- дополнительного технического контекста.

Пример:

```text
:sparkles: feat(search): add product results page

Adds a dedicated search results page using the existing site layout.

Refs: FP-123
```

## Запреты

Нельзя:

- писать commit message на русском языке;
- начинать сообщение без emoji;
- использовать emoji, не соответствующий смыслу изменения;
- смешивать несколько независимых изменений в одном commit message;
- использовать расплывчатые описания;
- предлагать commit message для изменений, которые не были проверены, если сборка или тесты завершились ошибкой.

## Финальный шаблон

```text
:emoji: type(scope)!: description

optional body

optional footer
```
