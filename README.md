# yafers
Yet Another FEis Registration System

[Concept scheme](https://drive.google.com/file/d/1B5P3XpQHNtHZ-zlgInDiAqQAC8CVnDc3/view?usp=sharing)

## Entities/tables

### Competition - Соревнование на феше 
Танец (соревнование) конкретном уровне или премьершип/чемпионат.
Соревнование входит в силлабус, может включать в себя несколько раундов.
- DanceId - ИД танца. Для соревнований, состоящих из нескольких раундов, - NULL
- Level - Уровень. Enum Beginner/Primary/Intermediate/Open
- Speed - Скорость танца. Для соревнований, состоящих из нескольких раундов, - NULL
- StartAge, EndAge - допустимые возраста для участия в соревновании. Не для формирования категорий на феше, а например для случая Jump-2-3, где можно участвовать танцорам младше 11 лет. Или International Championship, где танцору нужно быть младше 10 лет.
- IsTeam - true, если в танце участвуют 2 и более человек

### Dance - танец
Словарь танцев (требл джига, слип джига, требл рил, двуручник, модерн-сет, шоу и т.д.)

### Round
Раунд категории
- Name - Название раунда
- Number - Порядковый номер раунда
- DanceId - ИД танца (необязательно?)
- Description - Описание раунда

### SyllabusCompetition - Соревнования в силлабусе
- CompetitionId - ИД соревнования
- SyllabusId - ИД силлабуса
- Price - стоимость участия в соревновании
- RegistrationOrder - число для сортировки порядка регистрации танцев для одного танцора (например, сначал рил, потом лайт, слип, сингл и т.п., в конце кейли)

### Syllabus - Силлабус (шаблон силлабуса)
- Name - Название
- AssociationId - ИД ассоциации
- AdminFee - размер административного взноса
- IsTemplate

[The system will be here](http://yafers.ru/)
