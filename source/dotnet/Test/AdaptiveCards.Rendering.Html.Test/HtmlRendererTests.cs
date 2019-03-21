using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AdaptiveCards.Rendering.Html.Test
{
    [TestClass]
    public class HtmlRendererTests
    {
        [TestMethod]
        public void TextBlockRender_ParagraphElementStylesAdded()
        {
            var renderContext = new AdaptiveRenderContext(
                new AdaptiveHostConfig(),
                new AdaptiveElementRenderers<HtmlTag, AdaptiveRenderContext>());

            var textBlock = new AdaptiveTextBlock
            {
                Text = "first line\n\nsecond line",
            };

            var generatedHtml = TestHtmlRenderer.CallTextBlockRender(textBlock, renderContext).ToString();

            // From String

            // Generated HTML should have two <p> tags, with appropriate styles set.
            Assert.AreEqual(
                "<div class='ac-textblock' style='box-sizing: border-box;text-align: left;color: rgba(0, 0, 0, 1.00);line-height: 18.62px;font-size: 14px;font-weight: 400;white-space: nowrap;'><p style='margin-top: 0px;margin-bottom: 0px;width: 100%;text-overflow: ellipsis;overflow: hidden;'>first line</p><p style='margin-top: 0px;margin-bottom: 0px;width: 100%;text-overflow: ellipsis;overflow: hidden;'>second line</p></div>",
                generatedHtml);
        }

        [TestMethod]
        public void RichTextBlockRender_MultipleParagraphs()
        {
            var card = new AdaptiveCard("1.2")
            {
                Body = new System.Collections.Generic.List<AdaptiveElement>()
                {
                    new AdaptiveRichTextBlock()
                    {
                        Paragraphs = {
                            new AdaptiveRichTextBlock.AdaptiveParagraph() {
                                Inlines = {
                                    new AdaptiveRichTextBlock.AdaptiveParagraph.AdaptiveTextRun
                                    {
                                        Text = "Paragraph 1 Inline 1"
                                    },
                                    new AdaptiveRichTextBlock.AdaptiveParagraph.AdaptiveTextRun
                                    {
                                        Text = "Paragraph 1 Inline 2"
                                    }
                                }
                            },
                            new AdaptiveRichTextBlock.AdaptiveParagraph() {
                                Inlines = {
                                    new AdaptiveRichTextBlock.AdaptiveParagraph.AdaptiveTextRun
                                    {
                                        Text = "Paragraph 2 Inline 1"
                                    },
                                    new AdaptiveRichTextBlock.AdaptiveParagraph.AdaptiveTextRun
                                    {
                                        Text = "Paragraph 2 Inline 2"
                                    }
                                }
                            }
                        }
                    }
                }
            };

            var renderer = new AdaptiveCardRenderer();
            var result = renderer.RenderCard(card);
            var generatedHtml = result.Html.ToString();

            Assert.AreEqual(
                "<div class='ac-adaptivecard' style='width: 100%;background-color: rgba(255, 255, 255, 1.00);padding: 15px;box-sizing: border-box;justify-content: flex-start;'><div class='ac-richtextblock' style='box-sizing: border-box;text-align: left;white-space: nowrap;'><p style='margin-top: 0px;margin-bottom: 0px;width: 100%;text-overflow: ellipsis;overflow: hidden;'><span class='ac-textrun' style='color: rgba(0, 0, 0, 1.00);line-height: 18.62px;font-size: 14px;font-weight: 400;'>Paragraph 1 Inline 1</span><span class='ac-textrun' style='color: rgba(0, 0, 0, 1.00);line-height: 18.62px;font-size: 14px;font-weight: 400;'>Paragraph 1 Inline 2</span></p><p style='margin-top: 0px;margin-bottom: 0px;width: 100%;text-overflow: ellipsis;overflow: hidden;'><span class='ac-textrun' style='color: rgba(0, 0, 0, 1.00);line-height: 18.62px;font-size: 14px;font-weight: 400;'>Paragraph 2 Inline 1</span><span class='ac-textrun' style='color: rgba(0, 0, 0, 1.00);line-height: 18.62px;font-size: 14px;font-weight: 400;'>Paragraph 2 Inline 2</span></p></div></div>",
                generatedHtml);
        }

        [TestMethod]
        public void UnknownElementsRender()
        {
            var card = new AdaptiveCard("1.2")
            {
                Body =
                {
                    new AdaptiveUnknownElement()
                    {
                        Type = "Graph",
                        AdditionalProperties =
                        {
                            ["UnknownProperty1"] = "UnknownValue1"
                        }
                    }
                },
                Actions =
                {
                    new AdaptiveUnknownAction()
                    {
                        Type = "Action.Graph",
                        AdditionalProperties =
                        {
                            ["UnknownProperty2"] = "UnknownValue2"
                        }
                    }
                }
            };

            var renderer = new AdaptiveCardRenderer();
            var result = renderer.RenderCard(card);
            var generatedHtml = result.Html.ToString();

            Assert.AreEqual("<div class='ac-adaptivecard' style='width: 100%;background-color: rgba(255, 255, 255, 1.00);padding: 15px;box-sizing: border-box;justify-content: flex-start;'></div>", generatedHtml);
        }

        [TestMethod]
        public void ContainerStyleForegroundColors()
        {
            var hostConfig = new AdaptiveHostConfig();
            hostConfig.ContainerStyles.Emphasis.ForegroundColors = new ForegroundColorsConfig()
            {
                Default = new FontColorConfig("#FFcc3300")
            };

            var card = new AdaptiveCard("1.2")
            {
                Body = new System.Collections.Generic.List<AdaptiveElement>()
                {
                    new AdaptiveContainer()
                    {
                        Style = AdaptiveContainerStyle.Emphasis,
                        Items = new System.Collections.Generic.List<AdaptiveElement>()
                        {
                            new AdaptiveTextBlock()
                            {
                                Text = "container 1 -- emphasis style text"
                            },
                            new AdaptiveContainer()
                            {
                                Style = AdaptiveContainerStyle.Accent,
                                Items = new System.Collections.Generic.List<AdaptiveElement>()
                                {
                                    new AdaptiveTextBlock()
                                    {
                                        Text = "container 1.1 -- accent style text"
                                    }
                                }
                            },
                            new AdaptiveTextBlock()
                            {
                                Text = "container 1 -- emphasis style text"
                            }
                        }
                    },
                    new AdaptiveTextBlock()
                    {
                        Text = "default style text"
                    }
                }
            };

            var renderer = new AdaptiveCardRenderer(hostConfig);
            var result = renderer.RenderCard(card);
            var generatedHtml = result.Html.ToString();

            Assert.AreEqual(
                "<div class='ac-adaptivecard' style='width: 100%;background-color: rgba(255, 255, 255, 1.00);padding: 15px;box-sizing: border-box;justify-content: flex-start;'><div class='ac-container' style='padding-right: 15px;padding-left: 15px;padding-top: 15px;padding-bottom: 15px;background-color: rgba(0, 0, 0, 0.03);justify-content: flex-start;'><div class='ac-textblock' style='box-sizing: border-box;text-align: left;color: rgba(204, 51, 0, 1.00);line-height: 18.62px;font-size: 14px;font-weight: 400;white-space: nowrap;'><p style='margin-top: 0px;margin-bottom: 0px;width: 100%;text-overflow: ellipsis;overflow: hidden;'>container 1 -- emphasis style text</p></div><div class='ac-separator' style='height: 8px;'></div><div class='ac-container' style='padding-right: 15px;padding-left: 15px;padding-top: 15px;padding-bottom: 15px;background-color: #dce5f7;justify-content: flex-start;'><div class='ac-textblock' style='box-sizing: border-box;text-align: left;color: rgba(0, 0, 0, 1.00);line-height: 18.62px;font-size: 14px;font-weight: 400;white-space: nowrap;'><p style='margin-top: 0px;margin-bottom: 0px;width: 100%;text-overflow: ellipsis;overflow: hidden;'>container 1.1 -- accent style text</p></div></div><div class='ac-separator' style='height: 8px;'></div><div class='ac-textblock' style='box-sizing: border-box;text-align: left;color: rgba(204, 51, 0, 1.00);line-height: 18.62px;font-size: 14px;font-weight: 400;white-space: nowrap;'><p style='margin-top: 0px;margin-bottom: 0px;width: 100%;text-overflow: ellipsis;overflow: hidden;'>container 1 -- emphasis style text</p></div></div><div class='ac-separator' style='height: 8px;'></div><div class='ac-textblock' style='box-sizing: border-box;text-align: left;color: rgba(0, 0, 0, 1.00);line-height: 18.62px;font-size: 14px;font-weight: 400;white-space: nowrap;'><p style='margin-top: 0px;margin-bottom: 0px;width: 100%;text-overflow: ellipsis;overflow: hidden;'>default style text</p></div></div>",
                generatedHtml);
        }

        private class TestHtmlRenderer : AdaptiveCardRenderer
        {
            public TestHtmlRenderer(AdaptiveHostConfig config)
                : base(config)
            {
            }

            public static HtmlTag CallTextBlockRender(AdaptiveTextBlock element, AdaptiveRenderContext context)
            {
                return TextBlockRender(element, context);
            }

            public static HtmlTag CallChoiceSetInputRender(AdaptiveChoiceSetInput element, AdaptiveRenderContext context)
            {
                return ChoiceSetRender(element, context);
            }

            public static HtmlTag CallContainerRender(AdaptiveContainer element, AdaptiveRenderContext context)
            {
                return ContainerRender(element, context);
            }
        }

        [TestMethod]
        public void ChoiceSetInput()
        {
            var renderContext = new AdaptiveRenderContext(
                new AdaptiveHostConfig(),
                new AdaptiveElementRenderers<HtmlTag, AdaptiveRenderContext>());

            var dropdownList = new AdaptiveChoiceSetInput()
            {
                Id = "1",
                Value = "1,3",
                Style = AdaptiveChoiceInputStyle.Compact,
                Choices =
                {
                    new AdaptiveChoice()
                    {
                        Title = "Value 1",
                        Value = "1"
                    },
                    new AdaptiveChoice()
                    {
                        Title = "Value 2",
                        Value = "2"
                    },
                    new AdaptiveChoice()
                    {
                        Title = "Value 3",
                        Value = "3"
                    }
                }
            };

            var dropdownGeneratedHtml = TestHtmlRenderer.CallChoiceSetInputRender(dropdownList, renderContext).ToString();

            // Generated HTML should have an additional disabled and hidden option which is selected.
            Assert.AreEqual(
                "<select class='ac-input ac-multichoiceInput' name='1' style='width: 100%;'><option disabled='' hidden='' selected=''/><option value='1'>Value 1</option><option value='2'>Value 2</option><option value='3'>Value 3</option></select>",
                dropdownGeneratedHtml);
        }



        [TestMethod]
        public void BleedProperty()
        {
            var renderContext = new AdaptiveRenderContext(
                new AdaptiveHostConfig(),
                new AdaptiveElementRenderers<HtmlTag, AdaptiveRenderContext>());

            renderContext.ElementRenderers.Set<AdaptiveContainer>(TestHtmlRenderer.CallContainerRender);
            renderContext.RenderArgs.ParentStyle = AdaptiveContainerStyle.Default;

            var containerWithWorkingBleed = new AdaptiveContainer
            {
                Style = AdaptiveContainerStyle.Default,
                Items = new System.Collections.Generic.List<AdaptiveElement>
                {
                    new AdaptiveContainer()
                    {
                        Style = AdaptiveContainerStyle.Emphasis,
                        Bleed = true,
                        Items = new System.Collections.Generic.List<AdaptiveElement>()
                    }
                }
            };

            renderContext.RenderArgs.ParentStyle = AdaptiveContainerStyle.Default;

            var containerWithoutWorkingBleed = new AdaptiveContainer
            {
                Style = AdaptiveContainerStyle.Default,
                Items = new System.Collections.Generic.List<AdaptiveElement>
                {
                    new AdaptiveContainer()
                    {
                        Style = AdaptiveContainerStyle.Default,
                        Bleed = true,
                        Items = new System.Collections.Generic.List<AdaptiveElement>()
                    }
                }
            };

            var workingBleedHtml = TestHtmlRenderer.CallContainerRender(containerWithWorkingBleed, renderContext).ToString();
            var notWorkingBleedHtml = TestHtmlRenderer.CallContainerRender(containerWithoutWorkingBleed, renderContext).ToString();
            
            // Generated HTML should have an additional disabled and hidden option which is selected.
            Assert.AreEqual(
                "<div class='ac-container' style='background-color: rgba(255, 255, 255, 1.00);justify-content: flex-start;'><div class='ac-container' style='padding-right: 15px;padding-left: 15px;padding-top: 15px;padding-bottom: 15px;margin-right: -15px;margin-left: -15px;background-color: rgba(0, 0, 0, 0.03);justify-content: flex-start;'></div></div>",
                workingBleedHtml);

            Assert.AreEqual(
                "<div class='ac-container' style='background-color: rgba(255, 255, 255, 1.00);justify-content: flex-start;'><div class='ac-container' style='background-color: rgba(255, 255, 255, 1.00);justify-content: flex-start;'></div></div>",
                notWorkingBleedHtml);
        }
    }
}
