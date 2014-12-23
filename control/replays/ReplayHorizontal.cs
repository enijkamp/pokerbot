using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;

namespace PokerBot
{
    public class ReplayHorizontal : Replay
    {
        private static List<Point> points = new List<Point>()
        {
                 new Point(0,0),
                 new Point(0,0),
                 new Point(0,0),
                 new Point(0,0),
                 new Point(0,0),
                 new Point(0,0),
                 new Point(0,0),
                 new Point(0,0),
                 new Point(0,0),
                 new Point(0,0),
                 new Point(0,0),
                 new Point(0,0),
                 new Point(0,0),
                 new Point(0,0),
                 new Point(0,0),
                 new Point(0,0),
                 new Point(0,0),
                 new Point(0,0),
                 new Point(0,0),
                 new Point(0,0),
                 new Point(0,0),
                 new Point(0,0),
                 new Point(0,0),
                 new Point(0,0),
                 new Point(0,0),
                 new Point(0,0),
                 new Point(0,0),
                 new Point(0,0),
                 new Point(0,0),
                 new Point(0,0),
                 new Point(0,0),
                 new Point(0,0),
                 new Point(0,0),
                 new Point(0,0),
                 new Point(0,0),
                 new Point(0,0),
                 new Point(0,0),
                 new Point(0,0),
                 new Point(0,0),
                 new Point(0,0),
                 new Point(0,0),
                 new Point(0,0),
                 new Point(0,0),
                 new Point(0,0),
                 new Point(0,0),
                 new Point(0,0),
                 new Point(0,0),
                 new Point(0,0),
                 new Point(0,0),
                 new Point(0,0),
                 new Point(0,0),
                 new Point(0,0),
                 new Point(0,0),
                 new Point(0,0),
                 new Point(0,0),
                 new Point(0,0),
                 new Point(0,0),
                 new Point(0,0),
                 new Point(0,0),
                 new Point(0,1),
                 new Point(0,1),
                 new Point(0,1),
                 new Point(0,1),
                 new Point(0,1),
                 new Point(0,1),
                 new Point(0,1),
                 new Point(0,1),
                 new Point(0,1),
                 new Point(0,1),
                 new Point(0,1),
                 new Point(0,1),
                 new Point(0,1),
                 new Point(0,1),
                 new Point(0,1),
                 new Point(0,1),
                 new Point(0,1),
                 new Point(0,1),
                 new Point(0,1),
                 new Point(0,1),
                 new Point(0,1),
                 new Point(-1,1),
                 new Point(-1,1),
                 new Point(-1,1),
                 new Point(-1,1),
                 new Point(-1,1),
                 new Point(-1,1),
                 new Point(-1,1),
                 new Point(-1,1),
                 new Point(-1,1),
                 new Point(-1,1),
                 new Point(-1,1),
                 new Point(-1,1),
                 new Point(-1,1),
                 new Point(-1,1),
                 new Point(-1,1),
                 new Point(-1,1),
                 new Point(-1,1),
                 new Point(-1,1),
                 new Point(-1,1),
                 new Point(-1,1),
                 new Point(-1,1),
                 new Point(-1,1),
                 new Point(-1,1),
                 new Point(-1,1),
                 new Point(-1,1),
                 new Point(-1,1),
                 new Point(-1,1),
                 new Point(-1,1),
                 new Point(-1,1),
                 new Point(-1,1),
                 new Point(-1,1),
                 new Point(-2,1),
                 new Point(-2,1),
                 new Point(-2,1),
                 new Point(-2,1),
                 new Point(-2,1),
                 new Point(-2,1),
                 new Point(-2,2),
                 new Point(-2,2),
                 new Point(-2,2),
                 new Point(-2,2),
                 new Point(-3,2),
                 new Point(-3,2),
                 new Point(-3,2),
                 new Point(-3,2),
                 new Point(-3,2),
                 new Point(-3,2),
                 new Point(-4,2),
                 new Point(-4,2),
                 new Point(-4,2),
                 new Point(-4,2),
                 new Point(-4,2),
                 new Point(-4,2),
                 new Point(-4,2),
                 new Point(-4,2),
                 new Point(-4,1),
                 new Point(-4,1),
                 new Point(-4,1),
                 new Point(-5,0),
                 new Point(-5,0),
                 new Point(-5,0),
                 new Point(-5,0),
                 new Point(-5,-1),
                 new Point(-5,-1),
                 new Point(-5,-4),
                 new Point(-5,-6),
                 new Point(-5,-6),
                 new Point(-5,-6),
                 new Point(-5,-8),
                 new Point(-5,-8),
                 new Point(-5,-12),
                 new Point(-5,-13),
                 new Point(-5,-13),
                 new Point(-5,-13),
                 new Point(-5,-17),
                 new Point(-5,-19),
                 new Point(-5,-19),
                 new Point(-5,-23),
                 new Point(-5,-23),
                 new Point(-6,-25),
                 new Point(-6,-25),
                 new Point(-7,-30),
                 new Point(-8,-32),
                 new Point(-8,-32),
                 new Point(-8,-32),
                 new Point(-9,-37),
                 new Point(-9,-38),
                 new Point(-9,-38),
                 new Point(-9,-40),
                 new Point(-9,-40),
                 new Point(-9,-40),
                 new Point(-9,-43),
                 new Point(-9,-44),
                 new Point(-9,-44),
                 new Point(-10,-48),
                 new Point(-10,-48),
                 new Point(-10,-49),
                 new Point(-10,-49),
                 new Point(-10,-53),
                 new Point(-10,-53),
                 new Point(-11,-55),
                 new Point(-11,-55),
                 new Point(-11,-56),
                 new Point(-11,-56),
                 new Point(-11,-56),
                 new Point(-11,-57),
                 new Point(-11,-57),
                 new Point(-12,-59),
                 new Point(-12,-59),
                 new Point(-12,-59),
                 new Point(-12,-59),
                 new Point(-12,-59),
                 new Point(-12,-59),
                 new Point(-12,-59),
                 new Point(-12,-59),
                 new Point(-12,-59),
                 new Point(-12,-59),
                 new Point(-12,-59),
                 new Point(-12,-59),
                 new Point(-12,-59),
                 new Point(-11,-59),
                 new Point(-11,-59),
                 new Point(-11,-59),
                 new Point(-9,-59),
                 new Point(-8,-59),
                 new Point(-8,-59),
                 new Point(-5,-59),
                 new Point(-5,-59),
                 new Point(-3,-59),
                 new Point(-3,-59),
                 new Point(4,-59),
                 new Point(4,-59),
                 new Point(9,-58),
                 new Point(9,-58),
                 new Point(18,-57),
                 new Point(21,-56),
                 new Point(21,-56),
                 new Point(31,-55),
                 new Point(31,-55),
                 new Point(33,-54),
                 new Point(33,-54),
                 new Point(44,-54),
                 new Point(44,-54),
                 new Point(44,-54),
                 new Point(47,-54),
                 new Point(47,-54),
                 new Point(50,-53),
                 new Point(56,-53),
                 new Point(56,-53),
                 new Point(60,-53),
                 new Point(60,-53),
                 new Point(69,-53),
                 new Point(72,-53),
                 new Point(72,-53),
                 new Point(72,-53),
                 new Point(79,-53),
                 new Point(82,-53),
                 new Point(82,-53),
                 new Point(88,-53),
                 new Point(88,-53),
                 new Point(90,-53),
                 new Point(90,-53),
                 new Point(97,-53),
                 new Point(97,-53),
                 new Point(99,-53),
                 new Point(99,-53),
                 new Point(105,-54),
                 new Point(108,-54),
                 new Point(108,-54),
                 new Point(108,-54),
                 new Point(115,-55),
                 new Point(117,-55),
                 new Point(117,-55),
                 new Point(123,-55),
                 new Point(123,-55),
                 new Point(125,-55),
                 new Point(125,-55),
                 new Point(132,-55),
                 new Point(132,-55),
                 new Point(135,-56),
                 new Point(135,-56),
                 new Point(138,-56),
                 new Point(138,-56),
                 new Point(146,-56),
                 new Point(146,-56),
                 new Point(148,-56),
                 new Point(148,-56),
                 new Point(151,-56),
                 new Point(154,-56),
                 new Point(154,-56),
                 new Point(157,-56),
                 new Point(157,-56),
                 new Point(159,-56),
                 new Point(159,-56),
                 new Point(163,-56),
                 new Point(163,-56),
                 new Point(165,-56),
                 new Point(165,-56),
                 new Point(168,-56),
                 new Point(168,-56),
                 new Point(170,-56),
                 new Point(170,-56),
                 new Point(172,-56),
                 new Point(172,-56),
                 new Point(172,-56),
                 new Point(175,-57),
                 new Point(175,-57),
                 new Point(175,-57),
                 new Point(179,-57),
                 new Point(181,-57),
                 new Point(181,-57),
                 new Point(183,-57),
                 new Point(183,-57),
                 new Point(185,-57),
                 new Point(185,-57),
                 new Point(189,-57),
                 new Point(189,-57),
                 new Point(190,-57),
                 new Point(190,-57),
                 new Point(195,-57),
                 new Point(197,-57),
                 new Point(197,-57),
                 new Point(203,-57),
                 new Point(203,-57),
                 new Point(203,-57),
                 new Point(209,-57),
                 new Point(212,-58),
                 new Point(212,-58),
                 new Point(217,-58),
                 new Point(217,-58),
                 new Point(220,-58),
                 new Point(220,-58),
                 new Point(225,-58),
                 new Point(225,-58),
                 new Point(226,-58),
                 new Point(226,-58),
                 new Point(226,-58),
                 new Point(226,-58),
                 new Point(225,-58),
                 new Point(225,-58),
                 new Point(225,-58),
                 new Point(225,-58),
                 new Point(225,-58),
                 new Point(225,-58),
                 new Point(225,-58),
                 new Point(224,-58),
                 new Point(224,-58),
                 new Point(220,-57),
                 new Point(220,-57),
                 new Point(218,-56),
                 new Point(218,-56),
                 new Point(218,-56),
                 new Point(218,-56),
                 new Point(217,-56),
                 new Point(217,-56),
                 new Point(216,-56),
                 new Point(215,-56),
                 new Point(215,-56),
                 new Point(215,-56),
                 new Point(212,-56),
                 new Point(210,-56),
                 new Point(210,-56),
                 new Point(206,-56),
                 new Point(206,-56),
                 new Point(206,-56),
                 new Point(202,-56),
                 new Point(200,-55),
                 new Point(200,-55),
                 new Point(194,-55),
                 new Point(194,-55),
                 new Point(192,-55),
                 new Point(192,-55),
                 new Point(185,-55),
                 new Point(182,-55),
                 new Point(182,-55),
                 new Point(182,-55),
                 new Point(176,-55),
                 new Point(176,-55),
                 new Point(168,-55),
                 new Point(165,-54),
                 new Point(165,-54),
                 new Point(165,-54),
                 new Point(158,-54),
                 new Point(156,-54),
                 new Point(156,-54),
                 new Point(150,-53),
                 new Point(150,-53),
                 new Point(147,-53),
                 new Point(147,-53),
                 new Point(142,-51),
                 new Point(136,-50),
                 new Point(136,-50),
                 new Point(136,-50),
                 new Point(130,-48),
                 new Point(126,-47),
                 new Point(126,-47),
                 new Point(126,-47),
                 new Point(119,-45),
                 new Point(117,-45),
                 new Point(117,-45),
                 new Point(112,-43),
                 new Point(112,-43),
                 new Point(107,-43),
                 new Point(107,-43),
                 new Point(98,-39),
                 new Point(94,-39),
                 new Point(94,-39),
                 new Point(94,-39),
                 new Point(86,-36),
                 new Point(83,-36),
                 new Point(83,-36),
                 new Point(83,-36),
                 new Point(75,-34),
                 new Point(75,-34),
                 new Point(68,-31),
                 new Point(65,-30),
                 new Point(65,-30),
                 new Point(58,-28),
                 new Point(58,-28),
                 new Point(54,-26),
                 new Point(54,-26),
                 new Point(47,-24),
                 new Point(47,-24),
                 new Point(45,-23),
                 new Point(45,-23),
                 new Point(39,-22),
                 new Point(34,-20),
                 new Point(34,-20),
                 new Point(34,-20),
                 new Point(27,-18),
                 new Point(27,-18),
                 new Point(20,-15),
                 new Point(20,-15),
                 new Point(20,-15),
                 new Point(17,-14),
                 new Point(17,-14),
                 new Point(11,-12),
                 new Point(11,-12),
                 new Point(7,-11),
                 new Point(7,-11),
                 new Point(-3,-9),
                 new Point(-6,-8),
                 new Point(-6,-8),
                 new Point(-6,-8),
                 new Point(-14,-6),
                 new Point(-14,-6),
                 new Point(-22,-4),
                 new Point(-25,-3),
                 new Point(-25,-3),
                 new Point(-25,-3),
                 new Point(-32,-1),
                 new Point(-36,-1),
                 new Point(-36,-1),
                 new Point(-43,1),
                 new Point(-43,1),
                 new Point(-46,2),
                 new Point(-46,2),
                 new Point(-46,2),
                 new Point(-54,4),
                 new Point(-57,5),
                 new Point(-57,5),
                 new Point(-67,8),
                 new Point(-67,8),
                 new Point(-70,9),
                 new Point(-73,10),
                 new Point(-73,10),
                 new Point(-73,10),
                 new Point(-81,12),
                 new Point(-83,13),
                 new Point(-83,13),
                 new Point(-83,13),
                 new Point(-91,15),
                 new Point(-93,15),
                 new Point(-93,15),
                 new Point(-100,16),
                 new Point(-102,17),
                 new Point(-102,17),
                 new Point(-102,17),
                 new Point(-110,19),
                 new Point(-112,19),
                 new Point(-112,19),
                 new Point(-112,19),
                 new Point(-118,21),
                 new Point(-118,21),
                 new Point(-125,24),
                 new Point(-128,25),
                 new Point(-128,25),
                 new Point(-134,27),
                 new Point(-134,27),
                 new Point(-138,28),
                 new Point(-138,28),
                 new Point(-147,31),
                 new Point(-147,31),
                 new Point(-150,33),
                 new Point(-150,33),
                 new Point(-156,34),
                 new Point(-160,35),
                 new Point(-160,35),
                 new Point(-160,35),
                 new Point(-167,38),
                 new Point(-167,38),
                 new Point(-176,40),
                 new Point(-178,41),
                 new Point(-178,41),
                 new Point(-178,41),
                 new Point(-184,42),
                 new Point(-186,43),
                 new Point(-186,43),
                 new Point(-189,44),
                 new Point(-189,44),
                 new Point(-191,44),
                 new Point(-191,44),
                 new Point(-196,45),
                 new Point(-196,45),
                 new Point(-197,46),
                 new Point(-197,46),
                 new Point(-198,46),
                 new Point(-198,46),
                 new Point(-198,46),
                 new Point(-198,46),
                 new Point(-198,46),
                 new Point(-198,46),
                 new Point(-198,46),
                 new Point(-198,46),
                 new Point(-198,46),
                 new Point(-199,46),
                 new Point(-199,46),
                 new Point(-201,46),
                 new Point(-201,46),
                 new Point(-201,46),
                 new Point(-202,46),
                 new Point(-202,46),
                 new Point(-203,46),
                 new Point(-204,46),
                 new Point(-204,46),
                 new Point(-204,46),
                 new Point(-205,45),
                 new Point(-205,45),
                 new Point(-205,45),
                 new Point(-206,45),
                 new Point(-207,44),
                 new Point(-207,44),
                 new Point(-210,42),
                 new Point(-210,42),
                 new Point(-212,41),
                 new Point(-212,41),
                 new Point(-214,38),
                 new Point(-214,38),
                 new Point(-218,31),
                 new Point(-218,31),
                 new Point(-220,26),
                 new Point(-220,26),
                 new Point(-223,15),
                 new Point(-224,12),
                 new Point(-224,12),
                 new Point(-224,12),
                 new Point(-226,6),
                 new Point(-227,3),
                 new Point(-227,3),
                 new Point(-228,-4),
                 new Point(-228,-4),
                 new Point(-228,-4),
                 new Point(-229,-9),
                 new Point(-229,-10),
                 new Point(-229,-10),
                 new Point(-229,-15),
                 new Point(-229,-15),
                 new Point(-229,-18),
                 new Point(-229,-18),
                 new Point(-229,-23),
                 new Point(-229,-23),
                 new Point(-229,-23),
                 new Point(-229,-30),
                 new Point(-229,-30),
                 new Point(-229,-34),
                 new Point(-229,-34),
                 new Point(-229,-35),
                 new Point(-229,-35),
                 new Point(-229,-37),
                 new Point(-229,-37),
                 new Point(-229,-37),
                 new Point(-229,-37),
                 new Point(-229,-39),
                 new Point(-229,-39),
                 new Point(-229,-39),
                 new Point(-229,-39),
                 new Point(-229,-40),
                 new Point(-228,-42),
                 new Point(-228,-42),
                 new Point(-228,-42),
                 new Point(-228,-42),
                 new Point(-228,-42),
                 new Point(-228,-42),
                 new Point(-228,-42),
                 new Point(-228,-42),
                 new Point(-228,-43),
                 new Point(-228,-43),
                 new Point(-227,-43),
                 new Point(-227,-43),
                 new Point(-226,-44),
                 new Point(-226,-44),
                 new Point(-226,-44),
                 new Point(-226,-44),
                 new Point(-225,-44),
                 new Point(-225,-44),
                 new Point(-225,-45),
                 new Point(-224,-45),
                 new Point(-224,-45),
                 new Point(-221,-46),
                 new Point(-221,-46),
                 new Point(-221,-46),
                 new Point(-220,-47),
                 new Point(-218,-47),
                 new Point(-218,-47),
                 new Point(-215,-49),
                 new Point(-215,-49),
                 new Point(-213,-49),
                 new Point(-213,-49),
                 new Point(-207,-50),
                 new Point(-207,-50),
                 new Point(-204,-51),
                 new Point(-204,-51),
                 new Point(-195,-51),
                 new Point(-195,-51),
                 new Point(-195,-51),
                 new Point(-187,-51),
                 new Point(-187,-51),
                 new Point(-174,-51),
                 new Point(-174,-51),
                 new Point(-174,-51),
                 new Point(-160,-50),
                 new Point(-151,-50),
                 new Point(-151,-50),
                 new Point(-136,-49),
                 new Point(-136,-49),
                 new Point(-130,-49),
                 new Point(-130,-49),
                 new Point(-113,-48),
                 new Point(-108,-48),
                 new Point(-108,-48),
                 new Point(-108,-48),
                 new Point(-91,-47),
                 new Point(-88,-47),
                 new Point(-88,-47),
                 new Point(-82,-47),
                 new Point(-82,-47),
                 new Point(-79,-47),
                 new Point(-79,-47),
                 new Point(-72,-46),
                 new Point(-72,-46),
                 new Point(-66,-46),
                 new Point(-66,-46),
                 new Point(-64,-46),
                 new Point(-64,-46),
                 new Point(-59,-47),
                 new Point(-59,-47),
                 new Point(-55,-47),
                 new Point(-55,-47),
                 new Point(-48,-47),
                 new Point(-46,-47),
                 new Point(-46,-47),
                 new Point(-46,-47),
                 new Point(-39,-48),
                 new Point(-37,-48),
                 new Point(-37,-48),
                 new Point(-29,-48),
                 new Point(-29,-48),
                 new Point(-26,-48),
                 new Point(-26,-48),
                 new Point(-20,-48),
                 new Point(-20,-48),
                 new Point(-11,-48),
                 new Point(-11,-48),
                 new Point(-8,-48),
                 new Point(-8,-48),
                 new Point(1,-48),
                 new Point(6,-48),
                 new Point(6,-48),
                 new Point(6,-48),
                 new Point(14,-48),
                 new Point(18,-48),
                 new Point(18,-48),
                 new Point(27,-48),
                 new Point(27,-48),
                 new Point(33,-49),
                 new Point(33,-49),
                 new Point(36,-49),
                 new Point(36,-49),
                 new Point(43,-49),
                 new Point(43,-49),
                 new Point(46,-49),
                 new Point(46,-49),
                 new Point(52,-50),
                 new Point(55,-50),
                 new Point(55,-50),
                 new Point(58,-50),
                 new Point(58,-50),
                 new Point(61,-50),
                 new Point(61,-50),
                 new Point(64,-50),
                 new Point(64,-50),
                 new Point(70,-50),
                 new Point(70,-50),
                 new Point(73,-50),
                 new Point(73,-50),
                 new Point(79,-50),
                 new Point(79,-50),
                 new Point(81,-50),
                 new Point(81,-50),
                 new Point(85,-50),
                 new Point(88,-50),
                 new Point(88,-50),
                 new Point(88,-50),
                 new Point(94,-50),
                 new Point(95,-50),
                 new Point(95,-50),
                 new Point(95,-50),
                 new Point(100,-50),
                 new Point(101,-51),
                 new Point(101,-51),
                 new Point(101,-51),
                 new Point(103,-51),
                 new Point(103,-51),
                 new Point(106,-51),
                 new Point(108,-51),
                 new Point(111,-51),
                 new Point(111,-51),
                 new Point(111,-51),
                 new Point(117,-51),
                 new Point(120,-51),
                 new Point(120,-51),
                 new Point(126,-52),
                 new Point(126,-52),
                 new Point(129,-52),
                 new Point(129,-52),
                 new Point(135,-53),
                 new Point(135,-53),
                 new Point(137,-53),
                 new Point(137,-53),
                 new Point(143,-54),
                 new Point(146,-54),
                 new Point(146,-54),
                 new Point(146,-54),
                 new Point(153,-54),
                 new Point(155,-55),
                 new Point(155,-55),
                 new Point(155,-55),
                 new Point(161,-55),
                 new Point(164,-55),
                 new Point(164,-55),
                 new Point(169,-56),
                 new Point(169,-56),
                 new Point(172,-56),
                 new Point(172,-56),
                 new Point(176,-57),
                 new Point(179,-57),
                 new Point(179,-57),
                 new Point(179,-57),
                 new Point(184,-57),
                 new Point(184,-57),
                 new Point(191,-58),
                 new Point(194,-58),
                 new Point(194,-58),
                 new Point(194,-58),
                 new Point(200,-58),
                 new Point(202,-58),
                 new Point(202,-58),
                 new Point(209,-59),
                 new Point(209,-59),
                 new Point(212,-59),
                 new Point(212,-59),
                 new Point(218,-60),
                 new Point(220,-60),
                 new Point(220,-60),
                 new Point(220,-60),
                 new Point(226,-60),
                 new Point(229,-61),
                 new Point(229,-61),
                 new Point(231,-61),
                 new Point(231,-61),
                 new Point(231,-61),
                 new Point(231,-61),
                 new Point(231,-61),
                 new Point(231,-61),
                 new Point(232,-61),
                 new Point(232,-61),
                 new Point(232,-61),
                 new Point(232,-61),
                 new Point(232,-61),
                 new Point(232,-61),
                 new Point(232,-61),
                 new Point(232,-61),
                 new Point(233,-61),
                 new Point(233,-61),
                 new Point(233,-61),
                 new Point(233,-61),
                 new Point(233,-61),
                 new Point(233,-61),
                 new Point(233,-61),
                 new Point(233,-61),
                 new Point(233,-61),
                 new Point(233,-61),
                 new Point(233,-61),
                 new Point(233,-61),
                 new Point(233,-61),
                 new Point(233,-61),
                 new Point(233,-61),
                 new Point(233,-61),
                 new Point(233,-61),
                 new Point(232,-61),
                 new Point(231,-61),
                 new Point(231,-61),
                 new Point(231,-61),
                 new Point(228,-61),
                 new Point(228,-61),
                 new Point(228,-61),
                 new Point(223,-60),
                 new Point(223,-60),
                 new Point(216,-58),
                 new Point(216,-58),
                 new Point(209,-57),
                 new Point(209,-57),
                 new Point(207,-56),
                 new Point(207,-56),
                 new Point(203,-55),
                 new Point(203,-55),
                 new Point(203,-55),
                 new Point(203,-55),
                 new Point(199,-55),
                 new Point(198,-55),
                 new Point(198,-55),
                 new Point(198,-55),
                 new Point(195,-54),
                 new Point(195,-54),
                 new Point(195,-54),
                 new Point(195,-54),
                 new Point(195,-54),
                 new Point(194,-54),
                 new Point(194,-54),
                 new Point(194,-54),
                 new Point(194,-54),
                 new Point(194,-54),
                 new Point(194,-54),
                 new Point(194,-54),
                 new Point(194,-54),
                 new Point(193,-53),
                 new Point(191,-52),
                 new Point(191,-52),
                 new Point(191,-52),
                 new Point(191,-52),
                 new Point(190,-51),
                 new Point(190,-51),
                 new Point(189,-50),
                 new Point(189,-50),
                 new Point(185,-47),
                 new Point(185,-47),
                 new Point(184,-47),
                 new Point(184,-47),
                 new Point(181,-44),
                 new Point(181,-44),
                 new Point(177,-41),
                 new Point(177,-41),
                 new Point(176,-41),
                 new Point(176,-41),
                 new Point(173,-38),
                 new Point(173,-38),
                 new Point(173,-38),
                 new Point(169,-36),
                 new Point(169,-36),
                 new Point(168,-34),
                 new Point(168,-34),
                 new Point(168,-34),
                 new Point(168,-34),
                 new Point(164,-31),
                 new Point(164,-31),
                 new Point(162,-29),
                 new Point(162,-29),
                 new Point(159,-27),
                 new Point(159,-27),
                 new Point(157,-25),
                 new Point(155,-25),
                 new Point(155,-25),
                 new Point(151,-21),
                 new Point(151,-21),
                 new Point(151,-21),
                 new Point(146,-18),
                 new Point(143,-18),
                 new Point(143,-18),
                 new Point(143,-18),
                 new Point(137,-15),
                 new Point(134,-13),
                 new Point(134,-13),
                 new Point(127,-10),
                 new Point(127,-10),
                 new Point(124,-9),
                 new Point(124,-9),
                 new Point(119,-7),
                 new Point(117,-6),
                 new Point(117,-6),
                 new Point(117,-6),
                 new Point(113,-5),
                 new Point(111,-4),
                 new Point(111,-4),
                 new Point(110,-4),
                 new Point(110,-4),
                 new Point(110,-4),
                 new Point(108,-4),
                 new Point(107,-4),
                 new Point(107,-4),
                 new Point(105,-4),
                 new Point(105,-4),
                 new Point(105,-4),
                 new Point(105,-4),
                 new Point(103,-4),
                 new Point(103,-4),
                 new Point(101,-4),
                 new Point(99,-4),
                 new Point(99,-4),
                 new Point(99,-4),
                 new Point(96,-5),
                 new Point(94,-5),
                 new Point(94,-5),
                 new Point(91,-6),
                 new Point(91,-6),
                 new Point(89,-7),
                 new Point(89,-7),
                 new Point(83,-8),
                 new Point(83,-8),
                 new Point(78,-9),
                 new Point(78,-9),
                 new Point(76,-9),
                 new Point(76,-9),
                 new Point(69,-10),
                 new Point(69,-10),
                 new Point(67,-11),
                 new Point(67,-11),
                 new Point(62,-12),
                 new Point(60,-13),
                 new Point(60,-13),
                 new Point(60,-13),
                 new Point(54,-14),
                 new Point(48,-15),
                 new Point(48,-15),
                 new Point(38,-17),
                 new Point(35,-18),
                 new Point(35,-18),
                 new Point(35,-18),
                 new Point(26,-20),
                 new Point(26,-20),
                 new Point(19,-22),
                 new Point(16,-23),
                 new Point(16,-23),
                 new Point(16,-23),
                 new Point(7,-25),
                 new Point(4,-26),
                 new Point(4,-26),
                 new Point(4,-26),
                 new Point(1,-27),
                 new Point(-1,-27),
                 new Point(-1,-27),
                 new Point(-6,-29),
                 new Point(-6,-29),
                 new Point(-8,-29),
                 new Point(-8,-29),
                 new Point(-14,-31),
                 new Point(-14,-31),
                 new Point(-16,-31),
                 new Point(-16,-31),
                 new Point(-17,-32),
                 new Point(-17,-32),
                 new Point(-23,-33),
                 new Point(-23,-33),
                 new Point(-23,-33),
                 new Point(-28,-33),
                 new Point(-28,-33),
                 new Point(-36,-34),
                 new Point(-36,-34),
                 new Point(-38,-34),
                 new Point(-38,-34),
                 new Point(-45,-35),
                 new Point(-45,-35),
                 new Point(-48,-36),
                 new Point(-48,-36),
                 new Point(-53,-37),
                 new Point(-56,-37),
                 new Point(-56,-37),
                 new Point(-56,-37),
                 new Point(-63,-39),
                 new Point(-63,-39),
                 new Point(-67,-40),
                 new Point(-70,-40),
                 new Point(-70,-40),
                 new Point(-76,-41),
                 new Point(-76,-41),
                 new Point(-76,-41),
                 new Point(-76,-41),
                 new Point(-82,-42),
                 new Point(-82,-42),
                 new Point(-84,-42),
                 new Point(-84,-42),
                 new Point(-87,-43),
                 new Point(-87,-43),
                 new Point(-92,-43),
                 new Point(-92,-43),
                 new Point(-92,-43),
                 new Point(-98,-44),
                 new Point(-98,-44),
                 new Point(-98,-44),
                 new Point(-99,-44),
                 new Point(-99,-44),
                 new Point(-101,-44),
                 new Point(-103,-44),
                 new Point(-103,-44),
                 new Point(-105,-44),
                 new Point(-105,-44),
                 new Point(-107,-44),
                 new Point(-107,-44),
                 new Point(-111,-44),
                 new Point(-112,-44),
                 new Point(-112,-44),
        };

        public List<Point> getReplay()
        {
             return points;
        }

        public int getSpinWait()
        {
            return 500000;
        }
    }
}
