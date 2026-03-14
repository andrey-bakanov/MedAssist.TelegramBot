## Описание бота:


```
	@BotPhather /setdescription - добавить описание бота
	@BotPhather /editabout - добавить заголовок описания

	docker network create med-assist

	docker build -t medassist.worker.production -f MedAssist.TelegramBot.Worker/Dockerfile .
	
	docker stop medassist.worker.production
	docker rm -f medassist.worker.production 
	
	docker run --name medassist.worker.production --network med-assist  -t -d --env-file MedAssist.TelegramBot.Worker/.env medassist.worker.production
	
	docker run --name whisper --network med-assist -d -p 9000:9000 -e ASR_MODEL=base -e ASR_ENGINE=openai_whisper onerahmet/openai-whisper-asr-webservice:latest
```

---

**План:**

- [x] Регистрация
- [x] Изменение специализации 
- [x] Добавить работу с клиентами(пациентами) 
- [x] Обработка сообщений пользователя в процессах 
- [x] Мой профиль 
- [x] Логгирование http запросов
- [ ] Настройка serilog
- [x] Прикрутить web-api для админки 
	- [x] кеш 
- [x] Прикрутить web-api для отправки сообщений в LLM
	- [х] Отправка сообщений
	- [х] Отправка аудио (качество!)
	- [ ] Отправка изображений\файлов
	- [ ] Сохранять тип сообщения
- [ ] Получения ответа LLM
	- [x] Получение ответа в целом
	- [ ] Получение ответа чанками
- [ ] Обработка реакций на сообщения LLM
- [x] Прикрутить env файл
- [ ] Разбивать длинные ответы по 4096 заков по предложениям.


**Глобально:**
- [ ] Промпт под каждую специализацию
- [ ] Промпт должен предполагать диагнозы
- [ ] Промп не должен предлагать обращаться к врачу
- [ ] Никакой общей информации - кратко о заболевании и т.д.
- [ ] Динамическая смена специализации - Спросить как ...
- [ ] Структурированный диалог - воркфлоу для бота, например прием(сбор анамнеза, диагностика, назначения)
- [ ] Follow-up действия
