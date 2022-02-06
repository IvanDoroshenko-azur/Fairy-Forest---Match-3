using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Analytics;

public class Translator : MonoBehaviour
{
    static int langIndex = -1; // индекс языка: -1-еще не инициализирован, 0-англ, 1 - русский, 2 - франц. 3 - немец. 4 - китай.
    
    public static string Translate(string text_)
    {
        if (langIndex == -1) // начальная инициализация индекса языка при первом вызове
        {
            AnalyticsEvent.Custom("Translate", new Dictionary<string, object>
            {
               { "Language", Application.systemLanguage.ToString() }
            });
            switch (Application.systemLanguage.ToString())
            {
                
                case "English": langIndex = 0; break;
                case "Russian": langIndex = 1; break;
                case "Ukrainian": langIndex = 1; break;
                case "Belarusian": langIndex = 1; break;
                case "French": langIndex = 2; break;
                case "German": langIndex = 3; break;
                case "Chinese": langIndex = 4; break;
                case "ChineseSimplified": langIndex = 4; break;
                case "ChineseTraditional": langIndex = 4; break;
                default: langIndex = 0; break;
                    // продолжить для других языков ....
            }
        }

        for (int i = 0; i < labels.GetLength(0); i++)
        {
            if (text_ == labels[i, 0]) 
                 return labels[i, langIndex];
        }
        return text_;
    }


    // здесь будут все наши тексты
    static string[,] labels =
    {
    // англ.           // русс.             //франц.            //герман.           // китай.
    { "OK",              "OK",               "OK",               "OK",                "好"     },
    { "NO",              "Нет",              "AUCUN",            "KEIN",              "不"     },
    { "No Ads",          "Без рекламы",      "Pas De Publicité", "Keine Werbung",     "无广告" },
    { "Sale",            "Распродажа",       "Vente",            "Sale",              "销售"   },
    { "MENU",            "Меню",             "Menu",             "MENÜ",              "菜单"   },
    { "RATE",            "Оценить",          "TAUX",             "PREIS",             "点赞"   },
    { "SHOP",            "Магазин",          "BOUTIQUE",         "GESCHÄFT",          "商店"   },
    { "MAP",             "Карта",            "Map",              "ANZEIGEN",          "地图"   },
    { "NEXT",            "Следующий",        "PROCHAIN",         "NÄCHSTEN",          "下一页"  },

    { "Personal Offer",  "Персональное предложение",  "Offre Personnalisée", "Persönliches Angebot",  "个人优惠"      },
    { "Begin Pack",      "Начинающий Набор",   "Commencer Paquet", "Beginnen Pack",      "开始包"      },
    { "Medium Pack",     "Средний Набор",      "Medium Paquet",    "Mittlere Pack",      "中型包装"    },
    { "Profy Pack",      "Профи Набор",        "Profy Paquet",     "Profy Pack",         "虏漏虏"      },
    { "Happy Pack",      "Набор Счастливчика", "Happy Paquet",     "Happy Pack",         "快乐礼包"    },
    { "Сrazy Pack",      "Дикий нобор",        "Сrazy Paquet",     "Verrückte Pack",     "疯狂礼包"    },
    { "Great Pack",      "Великолепный нобор", "Magnifique Paquet", "Wunderschöne Pack", "最棒礼包"    },
    { "Big Pack",        "Большой Набор",      "Grand Paquet",     "Große Pack",         "大包"      },
    { "Mega Pack",       "Мега Набор",         "Mega Paquet",      "Mega Pack",          "大礼包"      },
    { "Royal Pack",      "Королевский Набор",  "Royal Paquet",     "Königliche Pack",    "皇家礼包"    },
    { "Forest Pack",     "Лесной Набор",       "La forêt Paquet",  "Forest Pack",        "森林礼包"    },
    { "Berry Pack",      "Ягодный Набор",      "Berry Paquet",     "Berry Pack",         "浆果礼包"    },
    { "King Pack",       "Царский Набор",      "Le Roi Paquet",    "King Pack",          "国王礼包"    },
    { "VIP Pack",        "VIP Набор",          "Paquet VIP",       "VIP Pack",           "会员礼包"    },
    { "Full Pack",       "Полный Набор",       "Full Paquet",      "Volle Pack",         "全服礼包"    },
    { "Giant Pack",      "Гигансткий Набор",   "Paquet Géant",     "Riesen Pack",        "超满足礼包"  },
    { "Honey Pack",      "Медовый Набор",      "Le Miel Paquet",   "Honig Pack",         "甜蜜礼包"    },

    { "Audio",           "Аудио",           "Audio",          "Audiodateien",      "音量"        },
    { "music",           "музыка",          "musical",        "Musik",             "音乐"        },
    { "easy",            "Лёгкий",          "facile",         "einfach",           "简单"        },
    { "Completed",       "Пройдено",        "Compléter",      "Abgeschlossen",     "已完成"      },
    { "Get",             "Получи",          "Obtenir",        "Bekommen",          "得到"        },
    { "h",               "ч",               "h",              "s",                 "小时"        },
    { "Buy",             "Купить",          "Acheter",        "Kaufen",            "购买"        },
    { "Get Score",       "Собрать очков",   "Obtenir Le Score",  "Get Score",      "获取分数"    },
    { "Your Score",      "Собрано очков",   "Votre Score",     "Ihre Punktzahl",    "你的分数"    },
    { "PROFILE",         "Профиль",         "PROFIL",          "PROFIL",           "玩家信息"    },
    { "CONTINUE",        "Продолжить",      "CONTINUER",       "WEITERHIN",        "继续"        },
    { "Profile",         "Профиль",         "Profil",          "Profil",           "玩家信息"    },
    { "SAVE",            "Сохранить",       "ENREGISTRER",     "SPEICHERN",        "救"          },
    { "CONNECT",         "войти",           "CONNECTER",       "VERBINDEN",        "连接"        },
    { "Settings",        "Настройки",       "Paramètre",       "Einstellung",      "设置"        },
    { "honey pack",      "Медовый",         "le miel pack",    "Honig-Packung",    "甜蜜礼包"     },
    { "Forest game",     "Волшебный лес",   "La forêt de jeu", "Wald-Spiel",       "仙女森林消除" },
    { "Congratulation!", "Поздравляю!",     "Félicitation!",    "Glückwunsch!",    "恭喜！"},
    { "You win!",        "Ты победил!",     "Tu as gagné!",  "Du hast gewonnen!", "你赢了" },
    { "Warning!",        "Внимание!",       "Attention!",       "Warnung!",            "警告！" },
    { "30 seconds left", "Осталось 30 сек.","30 secondes à gauche",  "30 Sekunden übrig",   "还剩30秒" },
    { "5 movesleft",     "Осталось 5 ходов", "5 movesleft",     "5 movesleft",     "只剩5步" },
    { "Moves",           "Ходы",            "Déplacer",         "Schritt",        "步数" },
    { "Score",           "Очки",            "Note",             "Ergebnis",        "分数" },
    { "Time",            "Время",           "Temps",            "Zeit",            "时间" },
    { "Level #",         "Уровень #",       "Niveau #",         "Ebene #",         "关卡#" },
    { "Level",           "Уровень",         "Niveau",           "Ebene",           "关卡" },
    { "Later",           "Позже",           "Plus tard",        "Später",          "后来" },
    { "Open",            "Открыть",         "Ouvrir",           "Öffnen",          "打开" },
    { "Mission",         "Задачи",          "Mission",          "Mission",         "任务" },
    { "Get boosters",    "Используй бустеры", "Obtenez des boosters", "Holen Sie Booster",  "获得道具" },
    { "Use this boosters", "Используй бустеры", "Utilisez ces boosters","Booster verwenden",  "使用助推器" },
    { "PLAY",           "Играть",           "JOUER",           "SPIELEN",           "玩" },
    { "Good Player",    "Хороший Игрок",    "Bon Joueur",      "Guter Spieler",     "干得漂亮" },
    { "EDIT",           "Изменить",         "MODIFIER",        "BEARBEITEN",        "编辑" },
    { "My Inventory",   "Мой инвентарь",    "Mon Inventaire",  "Mein Inventar",     "我的仓库" },
    { "Lifes Shop",     "Магазин",          "Mortes Boutique", "Leben Shop",        "生命商店" },
    { "Best Choice",    "Лучший выбор",     "Meilleur Choix",  "Beste Wahl",        "最棒的选择" },
    { "Watch video",    "Смотреть видео",   "Regarder la vidéo", "Watch video",     "观看视频" },
    { "Market",         "Рынок",            "Marché",          "Markt",             "市场" },
    { "off",            "выкл.",            "hors",            "off",               "关" },
    { "choice" ,        "выбор",            "choix" ,          "wahl" ,             "选择" },
    { "new offer",      "новое предложение", "la nouvelle offre", "neues Angebot", "新邀请" },
    { "Great",          "Великолепно!",     "Grand",           "Groß",              "棒" },
    { "Excellent",      "Отлично!",         "Excellent",        "Hervorragend",     "优秀" },
    { "Good",           "Хорошо!",          "Bon",              "Gut",             "真厉害 "},
    { "Out of moves!",  "Кончились ходы",   "Hors de mouvements!",  "Aus der bewegt!", "步数用尽" },
    { "mover for",      "ходов за",         "mover pour",       "mover für",       "移动" },
    { "and continue level", "и продолжи уровень", "et de continuer à niveau", "und weiter Ebene", "然后继续闯关" },
    { "RESTART",         "Заново",           "REDÉMARRER",       "STARTEN",          "重新开始" },
    { "REPLAY",          "Ещё раз",          "REPLAY",           "REPLAY",           "重玩" },
    { "RESUME",          "Продолжить",       "REPRENDRE",        "LEBENSLAUF",       "恢复" },
    { "PLAY ON",         "Продолжить",       "JOUER SUR",        "SPIELEN AUF",      "开始" },
    { "Failed",          "Провал",           "Échouer",          "Ausgefallen",      "失败了" },
    { "Time's up",       "Время вышло",      "Le temps est",     "Die Zeit ist vorbei",  "时间到了" },
    { "Tutorial",        "Подсказка",        "Tutoriel",         "Tutorial",         "教程" },
    { "CLEAR",           "Пoнятно",          "CLAIR",            "KLAR",             "清除" },
    { "Collect",         "Соберите",         "Recueillir",       "Sammeln",          "收集资料" },
    { "Fortuna",         "Фортуна",          "Fortuna",          "Fortuna",          "运" },
    { "GO!",             "Поехали!",         "Allez!",           "GEH!",             "去吧！" },
    { "Sorry!",          "Извините!",        "Pardon!",          "Sorry!",           "对不起！" },
    { "Not received",    "Не удалось загрузить",  "Non reçu",    "Nicht erhalten",   "未收到" },
    { "Pick up",         "Забрать",             "Ramasser",      "Abholen",          "拿起" },
    { "Popular",         "Популярное",          "Populaire",     "Beliebt",          "流行的" },
    { "Best price",      "Лучшая цена",         "Meilleur prix",        "Bester Preis",                "最好的价格" },
    { "Added 1 life",    "Добавлена 1 жизнь",   "Ajouté 1 vie",         "Hinzugefügt 1 Leben",         "拿起" },
    { "Collected Сoins", "Собранные монеты",    "Pièces collectées",    "Gesammelte Münzen",           "新增1条生命" },
    { "No chests!",      "Нет сундучков!",      "Pas de coffres!",      "Keine Truhen!",               "没有箱子!" },
    { "Open the chest",  "Открыть сундук",      "Ouvrez le coffre",     "Öffne die Truhe",             "打开胸部" },
                                                                                                                                      
    { "Congratulations on your purchase",   "Поздравляем с покупкой",            "Félicitations pour votre achat",                "Herzlichen Glückwunsch zum Kauf",           "祝贺您的购买"        },
    { "Look in the chest!",                 "Открывайте сундуки",                "Regardez dans la poitrine!",                    "Schau in die Brust",                        "看胸部"              },
    { "You can find a lot of bonuses",      "Получите множество бонусов",        "Vous pouvez trouver beaucoup de bonus",         "Sie können viele Boni finden",              "你可以找到很多奖金"   },
    { "Continue after video ads,",          "Продолжить после просмотра видео,", "Continuer après les annonces vidéo,",           "Weiter nach Video anzeigen,",               "观看视频后继续"       },
    { "with coins or restart",              "за монеты или начать заново",       "des pièces ou redémarrer",                      "für Münzen oder Neustart",                  "或再次"              },
    { "You have collected 3 stars",         "Вы собрали 3 звезды",               "Vous avez recueilli 3 étoiles",                 "Sie haben 3 Sterne gesammelt",              "你已经收集了3颗星,"   },
    { "You can open the chest on the map",  "Открыть сундук можно на карте",     "Vous pouvez ouvrir le coffre sur la carte",     "Sie können die Truhe auf der Karte öffnen", "可以在地图上打开宝箱" },
    { "and found a treasure!",              "и нашли клад!",                     "et trouvé un trésor!",                          "und fand einen Schatz!",                    "发现了宝藏！"        },
    { "Take 3 stars per level",             "Набери 3 звезды за уровень",        "Prenez 3 étoiles par niveau",                   "Nimm 3 Sterne pro level",                   "以每级3星，"         },
    { "and find a treasure chest!",         "и найдёшь сундук с кладом!",        "et trouver un coffre au trésor!",               "und finde eine Schatztruhe!",               "找到一个百宝箱!"     },
    { "Please rate the app!",               "Пожалуйста оцените игру",           "S'il vous plaît taux de l'application!",        "Bitte bewerten Sie die app!",               "请给这个游戏点赞"    },

    { "Booster Shop",                        "Магазин бустеров",                 "Booster Boutique",                              "Booster-Shop",                                 "道具商店" },
    { "Game active development!",            "Игра в активной разработке!",      "Développement actif du jeu!",                   "Spiel aktive Entwicklung!",                    "游戏进展顺利！" },
    { "You can also leave your feedback.",   "Вы так же можете оставить свой отзыв.",  "Vous pouvez aussi laisser vos commentaires.","Sie können auch Ihr feedback hinterlassen.", "您可以留下您的评论。" },
    { "Check it?",                           "Проверить?",                       "Le contrôler?",                                  "Überprüfen Sie es?",                           "检查？" },
    { "Game paused ...",                     "Игра на паузе...",                 "Le jeu en pause ...",                            "Spiel pausiert ...",                           "游戏暂停" },
    { "Sorry!",                              "Извините!",                        "Désolé!",                                       "Tut mir Leid!",                                 "对不起！" },
    { "You have no lifes.",                  "У вас кончились жизни.",           "Vous n'avez pas mortes.",                       "Du hast kein Leben.",                           "你没有生命了" },
    { "Succesfull!!!",                       "Успешно!!!",                       "Un vrai succès!!!",                             "Erfolgreich!!!",                                "赢了！" },
    { "Purchased successfull.",              "Поздравляем с покупкой!",          "Acheté avec succès.",                           "Gekauft erfolgreich.",                          "购买成功" },
    { "Received successfull.",               "Поздравляем с покупкой!",          "Reçu de succès.",                               "Erhielt erfolgreich.",                          "购买成功" },
    { "Not received.",                       "Не получилось.",                   "N'est pas reçu.",                               "Nicht erhalten.",                               "失败了" },
    { "Not enough coins.",                   "Не хватает монет.",                "Pas assez de pièces.",                          "Nicht genug Münzen.",                           "硬币不够" },
    { "Purchase failed.",                    "Покупка не удалась.",              "L'achat a échoué.",                             "Kauf fehlgeschlagen.",                          "购买失败" },
    // Конец уровней
    { "New levels will appear in the near future",    "В ближайшее время появятся новые уровни",    "De nouveaux niveaux seront bientôt disponibles",       "In naher Zukunft wird es neue Ebenen",      "新的水平将出现在不久的将来" },
    // Туториалы
    // Level#1
    { "Move it to collect 3 in a row",                                     "Передвинь, чтобы собрать 3 в ряд",                                    "Déplacez-vous pour recueillir 3 dans une rangée",                                               "Bewegen, um 3 in einer Reihe zu sammeln",                                              "移动它收集3在一排"                    },
    { "Collect 4 in a row and get a rocket",                               "Собери 4 в ряд и получи ракету",                                      "Recueillir 4 dans une rangée et obtenir une fusée",                                             "Sammle 4 in einer Reihe und bekomme eine Rakete",                                      "连续收集4个并获得火箭"                },
    { "Collect 5 in a row and get a color bomb",                           "Собери 5 в ряд и получи цветную бомбу",                               "Recueillir 5 dans une rangée et obtenir une bombe de couleur",                                  "Sammle 5 in einer Reihe und bekomme eine Farbbombe",                                   "连续收集5个，得到一个彩色炸弹"         },
    { "Collect 3 in a row vertically and horizontally and get dynamite",   "Собери 3 в ряд по вертикали и горизонтали и получи динамит",          "Recueillir 3 dans une rangée verticalement et horizontalement et obtenir de la dynamite",       "Sammle 3 in einer Reihe vertikal und horizontal und erhalten Dynamit",                 "垂直和水平连续收集3个并获得炸药"       },
    { "Click on the bomb or move to blow up a section of the map",         "Кликни на бомбу или передвинь, чтобы взорвать участок карты",         "Cliquez sur la bombe ou déplacez-la pour faire exploser une partie de la carte",                "Klicken Sie auf die Bombe oder bewegen, um den Bereich der Karte zu sprengen",                      "点击炸弹或移动它炸毁地图的一部分"              },
    { "Click on the rocket or move it to blow up a row",                   "Кликни на ракету или передвинь, чтобы взорвать ряд",                  "Cliquez sur une fusée ou déplacez-vous pour faire exploser une rangée",                         "Klicken Sie auf eine Rakete oder bewegen, um eine Reihe zu sprengen",                  "点击火箭或移动它炸毁一排"              },
    { "Click on the color bomb to blow up the same type of chips",         "Кликни на цветную бомбу, чтобы взорвать однотипные фишки",            "Cliquez sur la bombe de couleur pour faire sauter le même type de jetons",                      "Klicken Sie auf die Farbbombe, um die gleiche Art von Chips zu sprengen",              "点击彩色炸弹炸掉同类型的芯片"          },
    { "To break this item, collect 3 in a row next to it",                 "Чтобы разбить этот предмет собери рядом с ним 3 в ряд",               "Pour briser cet objet de recueillir à côté de lui 3 dans une rangée",                           "Um diesen Gegenstand zu zerschlagen, sammle neben ihm 3 in einer Reihe",               "要打破这个项目，在它旁边连续收集3个"    },
    { "To break this item, collect 3 in a row next to it 2 times",         "Чтобы разбить этот предмет собери рядом с ним 3 в ряд 2 раза",        "Pour briser cet objet de recueillir à côté de lui 3 dans une rangée 2 fois",                    "Um dieses Objekt zu zerschlagen, sammeln neben ihm 3 in einer Reihe 2 mal",            "要打破这个项目，在它旁边连续收集3次"    },
    { "To break this item, collect 3 in a row 3 times next to it",         "Чтобы разбить этот предмет собери рядом с ним 3 в ряр 3 раза",        "Pour casser cet objet, rassemblez-le à côté de lui 3 fois 3 fois",                              "Um diesen Gegenstand zu zerschlagen, sammle neben ihm 3 in einer Reihe 3 Mal",         "要打破这个项目，在它旁边连续收集3次"    },
    { "To break this item, collect 3 in a row on it",                      "Чтобы разбить этот предмет собери на нём 3 в ряд",                    "Pour briser cet objet recueillir sur elle 3 dans une rangée",                                   "Um diesen Gegenstand zu zerschlagen, sammle 3 in einer Reihe",                         "要打破这个项目，收集3在其上连续"        },
    { "To break this item, collect 3 in a row 2 times in it",              "Чтобы разбить этот предмет собери в нём 3 в ряд 2 раза",              "Pour briser cet objet recueillir en elle 3 dans une rangée 2 fois",                             "Um diesen Gegenstand zu zerschlagen, sammle ihn 3 in einer Reihe 2 Mal",               "要打破这个项目，收集3连续2次在它"       },
    { "To lower this item to the bottom, collect 3 in a row under it",     "Чтобы опустить в низ этот предмет собери под ним 3 в ряд",            "Pour abaisser le bas de cet objet de recueillir en dessous de lui 3 dans une rangée",           "Um den Boden des Gegenstandes zu senken, sammeln darunter 3 in einer Reihe",           "要将此项目降低到底部，请在其下方连续收集3个"    },
    // бустеры
    { "Use boosters. The color bomb booster removes a group of the selected chip.",
      "Используй бустеры. Бустер цветная бомба убирает группу выбранной фишки.",
      "Utilise les boosters. Booster bombe de couleur supprime un groupe de jetons sélectionnés.",
      "Benutze Booster. Die Farbbombe Booster entfernt die Gruppe der ausgewählten Chips.",
      "使用助推器。 色弹助推器取出所选芯片的一组。"  },

    { "Use boosters. The booster bomb removes the selected area of the map.",
      "Используй бустеры. Бустер бомба убирает выбранный участок карты.",
      "Utilise les boosters. La bombe de rappel supprime la partie sélectionnée de la carte.",
      "Benutze Booster. Booster Bombe entfernt den ausgewählten Abschnitt der Karte.",
      "使用助推器。 助推器炸弹移除地图的选定区域。"  },

    { "Use boosters. The mixer booster mixes up all the chips on the field.",
      "Используй бустеры. Бустер миксер перемешивает все фишки на поле.",
      "Utilise les boosters. Booster Mixer mélange tous les jetons sur le terrain.",
      "Benutze Booster. Mixer Booster mischt alle Chips auf dem Feld.",
      "使用助推器。 助推器混合器混合场上的所有芯片。"  },

    { "Use boosters. Booster hammer knocks out the selected chip on the field.",
      "Используй бустеры. Бустер  молоток выбивает выбранную фишку на поле.",
      "Utilise les boosters. Le marteau de rappel assomme la puce sélectionnée sur le terrain.",
      "BBenutze Booster. Booster Hammer klopft den ausgewählten Chip auf dem Feld.",
      "使用助推器。 飓风助推器混合了场上所有的筹码。"  },

    // Приветствие
    { "Hi! We should be on our way. Click on the level or Play!",          
      "Привет! Нам пора с тобой в путь. Жми на уровень или Играть!",         
      "Salut! Nous devrions être sur notre chemin. Cliquez sur le niveau ou jouer!",                   
      "Hallo! Wir sollten auf dem Weg sein. Klicken Sie auf die Ebene oder spielen!",         
      "嗨! 我们该走了 点击的水平还是玩！"              },

         
         
    // ...... ну и т.д., если языков больше, то в каждом блоке будет больше столбцов
  };
}
