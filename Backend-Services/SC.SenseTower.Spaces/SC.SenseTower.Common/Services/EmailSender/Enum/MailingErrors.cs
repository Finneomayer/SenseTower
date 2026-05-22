using System.ComponentModel;

namespace SC.SenseTower.Common.Services.EmailSender.Enum
{
    public enum MailingErrors
    {
        [Description("Временная ошибка. Попробуйте позднее (рекомендуемый таймаут - минута)")]
        retry_later,

        [Description("Содержимое вложения не является скалярным значением")]
        attachment_is_not_bytestring,

        [Description("Превышен допустимый размер вложения")]
        attachment_quota_error,

        [Description("Отсутствует тело письма")]
        body_empty,

        [Description("Тело письма превышает допустимый размер")]
        body_exceeds_length,

        [Description("Не указана тема письма")]
        empty_subject,

        [Description("Тема письма превышает допустимый размер")]
        subject_exceeds_length,

        [Description("Не указан обязательный параметр заголовка")]
        wrong_header_parameter,

        [Description("Указанный заголовок не поддерживается")]
        header_not_allowed,

        [Description("Недопустимый Email-адрес")]
        invalid_email,

        [Description("Не указано имя отправителя")]
        empty_sender_name,

        [Description("Недопустимый Email-адрес отправителя")]
        invalid_sender_email,

        [Description("Email-адрес отправителя не подтвержден")]
        unchecked_sender_email,

        [Description("Превышено количество Email-адресов")]
        cc_exceeded,

        [Description("Указанный язык не поддерживается системой")]
        unsupported_lang,

        [Description("Email данному адресату уже был отправлен")]
        has_been_sent,

        [Description("Вы забыли добавить ссылку отписки")]
        unsubscribe_link_missing,

        [Description("Адрес глобально отписан от рассылок")]
        unsubscribed_globally
    }
}
