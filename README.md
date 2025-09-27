#  AdService

Простой REST API сервис для хранения и поиска рекламных площадок по регионам.

##  Запуск

1. Клонируйте репозиторий:
   ```bash
   git clone https://github.com/uzzzakov/AdService.git
   cd AdService
   ```

2. Запустите сервис:
   ```bash
   dotnet run --project AdService --urls "http://localhost:7268"
   ```

3. Тестирование:
   ```bash
   dotnet test
   ```
---

##  REST API

### 1. Загрузка рекламных площадок из файла
```http
POST /upload
Content-Type: text/plain
```

Тело запроса (пример):
```
Яндекс.Директ:/ru
Ревдинский рабочий:/ru/svrd/revda,/ru/svrd/pervik
Газета уральских москвичей:/ru/msk,/ru/permobl,/ru/chelobl
Крутая реклама:/ru/svrd
```

Или в консоль:

```bash
Invoke-RestMethod -Uri "http://localhost:7268/upload" -Method Post -InFile "ads.txt" -ContentType "text/plain"
```

Ответ:
```json
"Data loaded"
```

### 2. Поиск площадок для локации
```http
GET /search?location=/ru/svrd/revda
```

Ответ:
```json
[
   "Крутая реклама",
   "Ревдинский рабочий",
   "Яндекс.Директ"
]
```

---

##  Тесты

Юнит-тесты написаны на **xUnit**. Запуск:
```bash
dotnet test
```