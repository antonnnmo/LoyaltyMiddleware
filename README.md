# LoyaltyMiddleware
1. Обертки над методами процессинга:
  Настроены обертки над метода calculate и confirm процессинга и методом info контакт-сервиса. Для добавления своего кода в эти обертки есть классы CalculateHandler, ConfirmHandler и InfoHandler.
  Для добавления новой обертки добавить метод в один из контроллеров в папке Controllers и свой Handler.
  
2. Классы для работы с БД процессинга: DBProvider для работы с БД процессинга и PersonalAreaDBProvider для работы с БД контакт-сервиса

3. Класс для отправки запросов в creation: CRMIntegrationProvider

4. В log4net.config можно настроить логирование запросов и ошибок
