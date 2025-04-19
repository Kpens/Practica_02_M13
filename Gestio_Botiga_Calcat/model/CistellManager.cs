using MongoDB.Bson;

namespace Gestio_Botiga_Calcat.model
{
    public class CistellManager
    {
        //public ObjectId Id_usu { get; set; }
        public ObjectId Id_cistell { get; set; }
        public Metode_enviamentMDB Metode_enviament { get; set; }
        public double Cost_enviament { get; set; }
        public ExtendedObservableCollection<Prod_select> Prod_select_no_logged { get; set; }
        public ExtendedObservableCollection<Prod_select> Prod_select_logged { get; set; }
        public List<Prod_select> Prod_select_comprar{ get; set; }


        public void loginOk(bool enfactura)
        {
            if(enfactura)
            {
                Prod_select_logged = Prod_select_no_logged;
                Global.mdbService.ActualizarCistell();
                return;
            }
            CistellMDB cistellBd;
            if ((cistellBd = Global.mdbService.GetCistell(Global.Usuari.Id))!=null) { 
                Prod_select_logged = cistellBd.Prod_select;
                if(Id_cistell == ObjectId.Empty)
                {
                    Id_cistell = cistellBd.Id;
                    Cost_enviament = cistellBd.Cost_enviament;
                }
                if(Metode_enviament.Id == ObjectId.Empty)
                {
                    Global.mdbService.GetMetode_enviament(cistellBd.Metode_enviament);
                    Metode_enviament = Global.mdbService.GetMetode_enviament(cistellBd.Metode_enviament);
                }

                if (Prod_select_no_logged != null)
                {
                    foreach (Prod_select prodWeb in Prod_select_no_logged)
                    {
                        int productesIguals = cistellBd.Prod_select.Where(prodBd => prodWeb.Id == prodBd.Id && prodWeb.Estoc_id == prodBd.Estoc_id).Count();

                        if (productesIguals == 0)
                        {
                            cistellBd.Prod_select.Add(prodWeb);
                            Prod_select_logged.Add(prodWeb);
                        }

                    }
                }
            }
            else
            {
                Prod_select_logged = Prod_select_no_logged;

            }


            Global.mdbService.ActualizarCistell();
        }

        public void AddProd(Prod_select prod)
        {
            if (Global.Usuari != null)
            {
                if (Prod_select_logged == null)
                {
                    Prod_select_logged = new ExtendedObservableCollection<Prod_select>();
                }
                else
                {
                    int productesIguals = Prod_select_logged.Where(prodBd => prod.Id == prodBd.Id && prod.Estoc_id == prodBd.Estoc_id).Count();
                    if (productesIguals > 0)
                    {
                        return;
                    }
                }
                 Prod_select_logged.Add(prod);
            }
            else
            {
                if (Prod_select_no_logged == null)
                {
                    Prod_select_no_logged = new ExtendedObservableCollection<Prod_select>();
                }
                else
                {
                    int productesIguals = Prod_select_no_logged.Where(prodBd => prod.Id == prodBd.Id && prod.Estoc_id == prodBd.Estoc_id).Count();
                    if (productesIguals > 0)
                    {
                        return;
                    }
                }
                Prod_select_no_logged.Add(prod);
            }
        }

        public void RemoveProd(Prod_select prod)
        {
            if (Global.Usuari != null)
            {
                if (Prod_select_logged != null)
                {
                    Prod_select_logged.Remove(prod);
                }
            }
            else
            {
                if (Prod_select_no_logged != null)
                {
                    Prod_select_no_logged.Remove(prod);
                }
            }
        }

        public int GetQtProd()
        {
            if (Global.Usuari != null)
            {
                if (Prod_select_logged != null)
                {
                    return Prod_select_logged.Count;
                }
            }
            else
            {
                if (Prod_select_no_logged != null)
                {
                    return Prod_select_no_logged.Count;
                }
            }
            return 0;
        }

        public ExtendedObservableCollection<Prod_select> GetLlistaProds()
        {
            if (Global.Usuari != null)
            {
                if (Prod_select_logged != null)
                {
                    return Prod_select_logged;
                }
            }
            else
            {
                if (Prod_select_no_logged != null)
                {
                    return Prod_select_no_logged;
                }
            }
            return new ExtendedObservableCollection<Prod_select>();
        }

        public CistellMDB GetCistell()
        {
            CistellMDB cistell = new CistellMDB();
            cistell.Prod_select = new ExtendedObservableCollection<Prod_select>();
            if (Global.Usuari != null)
            {
                if (Prod_select_logged == null)
                {
                    Prod_select_logged = new ExtendedObservableCollection<Prod_select>();
                }
                cistell.Prod_select = new ExtendedObservableCollection<Prod_select>(Prod_select_logged);
                cistell.Id_usu = Global.Usuari.Id;
                cistell.Cost_enviament = Cost_enviament;
                cistell.Metode_enviament = Metode_enviament.Id;
            }
            else
            {

                if (Prod_select_no_logged == null)
                {
                    Prod_select_no_logged = new ExtendedObservableCollection<Prod_select>();
                }
                cistell.Prod_select = new ExtendedObservableCollection<Prod_select>(Prod_select_no_logged);
            }
            return cistell;
        }

    }
}