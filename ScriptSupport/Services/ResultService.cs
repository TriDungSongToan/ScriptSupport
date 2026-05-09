using System.Windows;
using System.Windows.Documents;
using Scrapiyard.Core.Models;
using Scrapiyard.Core.Services;
using ScriptSupport.Models;
using ScriptSupport.States;
using ScriptSupport.Interfaces;

namespace ScriptSupport.Services
{
    public class ResultService : IResultInterface
    {
        private readonly ICardInterface _cardService;
        private readonly IImageCardInterface _imageCardService;
        private readonly ResultState _resultState;
        private readonly UIConfigState UIConfig;

        public ResultService(ICardInterface cardService, IImageCardInterface imageCardService,
            ResultState resultState, UIConfigState uIConfig)
        {
            _cardService = cardService;
            _imageCardService = imageCardService;
            _resultState = resultState;
            UIConfig = uIConfig;
        }

        public void BuildResultCardDocument(CardText? card)
        {
            _resultState.ResultCardDocument.Text = card == null
                ? string.Empty
                : $"{card.name?.Trim()}\t{card.id}\n{card.desc?.Trim()}";

            FlowDocument doc = new FlowDocument();
            if (card == null)
            {
                _resultState.ResultCardDocument1 = doc;
                return;
            }

            var para = new Paragraph();
            para.Inlines.Add(new Run(card.name?.Trim()) { Foreground = UIConfig.Foreground, FontWeight = FontWeights.Thin });
            para.Inlines.Add(new Run(" — ") { Foreground = UIConfig.Foreground, FontWeight = FontWeights.Normal});
            para.Inlines.Add(new Run(card.id.ToString()) { Foreground = UIConfig.ThemeColor, FontWeight = FontWeights.Normal });
            para.Inlines.Add(new LineBreak());
            para.Inlines.Add(new Run(card.desc?.Trim()) { Foreground = UIConfig.Foreground, FontWeight = FontWeights.Thin });

            doc.Blocks.Add(para);
            _resultState.ResultCardDocument1 = doc;
        }
        public void BuildResultScrapiyardDocument(CompletionSymbol? completionSymbol)
        {
            _resultState.ResultScrapiyardDocument.Text =
                completionSymbol == null ? string.Empty : SymbolDescriptionBuilder.Build(completionSymbol);
        }
        public void BuildResultID(CardText? card)
        {
            if (card == null || card.id == 0) _resultState.ResultID = new();
            else BuildResultID(card.id);
        }
        public void BuildResultID(ulong cardID)
        {
            var listID = _cardService.GetListIDByID(cardID);
            _resultState.ResultID = listID;
        }
        public void BuildResultCardTexts(CardText? card)
        {
            if (card == null || card.id == 0) _resultState.ResultCardTexts = new();
            else BuildResultCardTexts(card.id);
        }
        public void BuildResultCardTexts(ulong cardID)
        {
            var listID = _cardService.GetListIDByID(cardID);
            if (listID == null || !listID.Any())
            {
                _resultState.ResultCardTexts = new();
                return;
            }

            List<CardText> listTexts = new List<CardText>();
            foreach (var ID in listID)
            {
                var list = _cardService.GetListCardTextByID(ID);
                if (list != null && list.Any()) listTexts.AddRange(list);
            }
            _resultState.ResultCardTexts = new(listTexts);
        }
        public void BuildResultImageCards(CardText? card)
        {
            if (card == null || card.id == 0) _resultState.ResultImageCards = new();
            else BuildResultImageCards(card.id);
        }
        public void BuildResultImageCards(ulong cardID)
        {
            //var listID = _resultState.ResultID ?? _cardService.GetListIDByID(cardID);
            var listID = _cardService.GetListIDByID(cardID);
            if (listID == null || !listID.Any())
            {
                _resultState.ResultImageCards = new();
                return;
            }

            BuildResultImageCards(listID);
        }
        public void BuildResultImageCards(List<ulong> ids)
        {
            var list = new List<FileItem>();
            foreach (var id in ids)
            {
                var listFile = GetListImageCards(id);
                list.AddRange(listFile);
            }
            _resultState.ResultImageCards = new(list);
        }
        public List<FileItem> GetListImageCards(ulong id)
        {
            var imagePaths = _imageCardService.GetImagePath(id);
            if (imagePaths == null || !imagePaths.Any()) return new();
            else
            {
                var list = new List<FileItem>();
                foreach (var file in imagePaths)
                {
                    list.Add(new FileItem { FullPath =  file });
                }
                return list;
            }
        }
        public void BuildResultCardData(CardText? card)
        {
            if (card == null || card.id == 0) _resultState.ResultCardData = null;
            else BuildResultCardData(card.id);
        }
        public void BuildResultCardData(ulong id)
        {
            var cardData = _cardService.GetCardDataByID(id);
            _resultState.ResultCardData = cardData;
        }
        public void BuildResultScrapiyard(CompletionSymbol? completionSymbol)
        {
            _resultState.ResultScrapiyard = completionSymbol;
        }

    }
}
