namespace FortsPositionsViewer
{
    public static class EnumExtentions
    {
        public static string GetText(this DeriativeNames value)
        {
            switch (value)
            {
                case DeriativeNames.Si:
                    return "Si";
               case DeriativeNames.BR:
                    return "Brent";
               case DeriativeNames.SBRF:
                    return "Sber";
               case DeriativeNames.RTS:
                    return "RTS";
               case DeriativeNames.GAZR:
                    return "Gazprom";
               case DeriativeNames.GOLD:
                    return "Gold";
               case DeriativeNames.VTBR:
                    return "Bank VTB";
               case DeriativeNames.GMKR:
                    return "GMK";
               case DeriativeNames.CHMF:
                    return "CHMF";
               case DeriativeNames.ROSN:
                    return "Rosneft";
               case DeriativeNames.MXI:
                    return "Micex Mini";
               case DeriativeNames.MTSI:
                    return "MTSI";
               case DeriativeNames.MGNT:
                    return "Magnit";
               case DeriativeNames.SNGP:
                    return "Surgut";
               case DeriativeNames.UUAH:
                    return "Ukrane Grivna";
            }

            return value.ToString();
        }
    }
}
