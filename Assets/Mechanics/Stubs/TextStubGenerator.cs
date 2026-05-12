using System;
using System.Collections.Generic;
using System.Text;

namespace Stubs
{
    public static class TextStubGenerator
    {
        public static readonly List<string> Titles1 = new List<string>()
            {
                "Новый", "Крутой", "Эпический", "Неизведанный"
            };
        public static readonly List<string> Titles2 = new List<string>()
            {
                "Рассвет", "Космос", "Успех", "Фильм", "Креатив"
            };
        public static readonly List<string> Spaces = new List<string>()
            {
                "Лекторий", "Кинотеатр", "Переговорная", "Галерея", "Miron1", "Стендап"
            };
        public static readonly List<string> Sentences = new List<string>()
            {
                "Товарищи! Постоянный количественный рост и сфера нашей активности требуют определения и " +
                "уточнения позиций, занимаемых участниками в отношении поставленных задач.",
                "Значимость этих проблем настолько очевидна, что консультация с широким активом позволяет оценить значение " +
                "дальнейших направлений развития.",
                "Таким образом начало повседневной работы по формированию позиции требуют от нас анализа форм развития.",
                "С другой стороны консультация с широким активом представляет собой интересный эксперимент проверки систем массового участия.",
                "Не следует, однако забывать, что дальнейшее развитие различных форм деятельности требуют определения " +
                "и уточнения новых предложений.",
                "Равным образом постоянный количественный рост и сфера нашей активности позволяет оценить значение систем массового участия.",
                "Каждый из нас понимает очевидную вещь: граница обучения кадров способствует подготовке " +
                "и реализации анализа существующих паттернов поведения."
            };
        public static readonly List<string> Images = new List<string>()
            {
                "https://i.pinimg.com/564x/f0/31/88/f0318884e63fe7120e8146d3deb66cdd.jpg",
                "https://apprecs.org/ios/images/app-icons/256/59/415918681.jpg",
                "https://www.lovethispic.com/uploaded_images/117570-Beautiful-View.jpg",
                "https://i.pinimg.com/564x/f0/31/88/f0318884e63fe7120e8146d3deb66cdd.jpg",
                "https://media-cdn.tripadvisor.com/media/photo-s/0f/f8/63/3d/morning-coffee-and-the.jpg",
            };

        private static Random _random = new();

        public static string GetFishString(List<string> strings)
        {
            if (strings == null || strings.Count == 0)
            {
                return string.Empty;
            }
            return strings[_random.Next(0, strings.Count - 1)];
        }

        public static string GetFishText(List<string> strings, int minSentenсeCount, int maxSentenсeCount)
        {
            if (strings == null || strings.Count == 0)
            {
                return string.Empty;
            }

            int sentenceCount = _random.Next(minSentenсeCount, maxSentenсeCount);

            StringBuilder stringBuilder = new();
            for (int i = 0; i < sentenceCount; i++)
            {
                stringBuilder.Append(strings[_random.Next(0, strings.Count - 1)]);
            }

            return stringBuilder.ToString();
        }
    }
}
