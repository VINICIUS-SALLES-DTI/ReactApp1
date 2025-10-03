import { useState, useEffect } from "react";
import { Plus, Calculator, Edit, Trash2, ChevronRight, Loader2 } from "lucide-react";
import { Button } from "@/components/ui/button";
import { Card, CardContent, CardDescription, CardHeader, CardTitle } from "@/components/ui/card";
import { Dialog, DialogContent, DialogDescription, DialogHeader, DialogTitle, DialogTrigger } from "@/components/ui/dialog";
import { Input } from "@/components/ui/input";
import { Label } from "@/components/ui/label";
import { useToast } from "@/hooks/use-toast";
import { Badge } from "@/components/ui/badge";
import { apiService, Traco, TracoDetalhes, ComponenteDto } from "@/services/api";

const Tracos = () => {
  const { toast } = useToast();
  const [tracos, setTracos] = useState<Traco[]>([]);
  const [tracosDetalhes, setTracosDetalhes] = useState<Record<number, TracoDetalhes>>({});
  const [custos, setCustos] = useState<Record<number, number>>({});
  const [loading, setLoading] = useState(true);
  const [isDialogOpen, setIsDialogOpen] = useState(false);
  const [selectedTraco, setSelectedTraco] = useState<Traco | null>(null);
  const [formData, setFormData] = useState({
    nome: "",
    resistenciaFck: "",
    slump: "",
  });
  const [submitting, setSubmitting] = useState(false);
  const [calculatingCost, setCalculatingCost] = useState<number | null>(null);

  useEffect(() => {
    loadTracos();
  }, []);

  const loadTracos = async () => {
    try {
      setLoading(true);
      const data = await apiService.getTracos();
      setTracos(data);
      
      // Carregar detalhes dos traços
      const detalhesPromises = data.map(async (traco) => {
        try {
          const detalhes = await apiService.getTracoDetalhes(traco.id);
          return { id: traco.id, detalhes };
        } catch {
          return null;
        }
      });
      
      const detalhesResults = await Promise.all(detalhesPromises);
      const detalhesMap: Record<number, TracoDetalhes> = {};
      
      detalhesResults.forEach((result) => {
        if (result) {
          detalhesMap[result.id] = result.detalhes;
        }
      });
      
      setTracosDetalhes(detalhesMap);
    } catch (error) {
      toast({
        title: "Erro ao carregar traços",
        description: "Não foi possível carregar a lista de traços.",
        variant: "destructive",
      });
    } finally {
      setLoading(false);
    }
  };

  const calculateCost = async (traco: Traco) => {
    try {
      setCalculatingCost(traco.id);
      const custoData = await apiService.calcularCustoTraco(traco.id);
      
      toast({
        title: "Custo Calculado",
        description: `O custo total do traço é R$ ${custoData.custoTotalPorM3.toFixed(2)} por m³`,
      });
      
      // Atualizar o traço com o custo calculado
      setCustos(prev => ({
        ...prev,
        [traco.id]: custoData.custoTotalPorM3
      }));
    } catch (error) {
      toast({
        title: "Erro ao calcular custo",
        description: "Não foi possível calcular o custo do traço.",
        variant: "destructive",
      });
    } finally {
      setCalculatingCost(null);
    }
  };

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    setSubmitting(true);
    
    try {
      const tracoData = {
        nome: formData.nome,
        resistenciaFck: parseFloat(formData.resistenciaFck),
        slump: parseFloat(formData.slump),
        componentes: [] as Array<{ materialId: number; quantidade: number }>,
      };
      
      await apiService.createTraco(tracoData);
      toast({
        title: "Traço cadastrado",
        description: "O traço foi cadastrado com sucesso.",
      });
      
      resetForm();
      await loadTracos();
    } catch (error) {
      toast({
        title: "Erro",
        description: "Não foi possível cadastrar o traço.",
        variant: "destructive",
      });
    } finally {
      setSubmitting(false);
    }
  };

  const handleDelete = async (id: number) => {
    try {
      await apiService.deleteTraco(id);
      toast({
        title: "Traço excluído",
        description: "O traço foi excluído com sucesso.",
        variant: "destructive",
      });
      await loadTracos();
    } catch (error) {
      toast({
        title: "Erro",
        description: "Não foi possível excluir o traço.",
        variant: "destructive",
      });
    }
  };

  const resetForm = () => {
    setFormData({ nome: "", resistenciaFck: "", slump: "" });
    setIsDialogOpen(false);
  };

  return (
    <div className="container mx-auto p-6 space-y-6">
      <div className="flex items-center justify-between">
        <div>
          <h1 className="text-3xl font-bold text-foreground">Traços de Concreto</h1>
          <p className="text-muted-foreground mt-1">Gerencie e calcule os custos dos traços</p>
        </div>
        
        <Dialog open={isDialogOpen} onOpenChange={setIsDialogOpen}>
          <DialogTrigger asChild>
            <Button className="gap-2">
              <Plus className="h-4 w-4" />
              Novo Traço
            </Button>
          </DialogTrigger>
          <DialogContent>
            <DialogHeader>
              <DialogTitle>Novo Traço</DialogTitle>
              <DialogDescription>
                Cadastre um novo traço de concreto
              </DialogDescription>
            </DialogHeader>
            <form onSubmit={handleSubmit} className="space-y-4">
              <div className="space-y-2">
                <Label htmlFor="nome">Nome do Traço</Label>
                <Input
                  id="nome"
                  value={formData.nome}
                  onChange={(e) => setFormData({ ...formData, nome: e.target.value })}
                  placeholder="Ex: Traço 1:3:4"
                  required
                />
              </div>
              <div className="space-y-2">
                <Label htmlFor="resistenciaFck">Resistência FCK (MPa)</Label>
                <Input
                  id="resistenciaFck"
                  type="number"
                  step="0.1"
                  value={formData.resistenciaFck}
                  onChange={(e) => setFormData({ ...formData, resistenciaFck: e.target.value })}
                  placeholder="Ex: 25"
                  required
                />
              </div>
              <div className="space-y-2">
                <Label htmlFor="slump">Slump (cm)</Label>
                <Input
                  id="slump"
                  type="number"
                  step="0.1"
                  value={formData.slump}
                  onChange={(e) => setFormData({ ...formData, slump: e.target.value })}
                  placeholder="Ex: 10"
                  required
                />
              </div>
              <div className="flex gap-2 justify-end pt-4">
                <Button type="button" variant="outline" onClick={resetForm} disabled={submitting}>
                  Cancelar
                </Button>
                <Button type="submit" disabled={submitting}>
                  {submitting && <Loader2 className="mr-2 h-4 w-4 animate-spin" />}
                  Cadastrar
                </Button>
              </div>
            </form>
          </DialogContent>
        </Dialog>
      </div>

      {loading ? (
        <div className="flex items-center justify-center p-8">
          <Loader2 className="h-8 w-8 animate-spin" />
          <span className="ml-2">Carregando traços...</span>
        </div>
      ) : (
        <div className="grid grid-cols-1 md:grid-cols-2 gap-4">
          {tracos.map((traco) => (
          <Card key={traco.id} className="hover:shadow-lg transition-shadow">
            <CardHeader>
              <CardTitle className="flex items-center justify-between">
                {traco.nome}
                <div className="flex gap-2">
                  <Button
                    size="icon"
                    variant="ghost"
                    onClick={() => calculateCost(traco)}
                    disabled={calculatingCost === traco.id}
                  >
                    {calculatingCost === traco.id ? (
                      <Loader2 className="h-4 w-4 animate-spin" />
                    ) : (
                      <Calculator className="h-4 w-4" />
                    )}
                  </Button>
                  <Button
                    size="icon"
                    variant="ghost"
                    onClick={() => handleDelete(traco.id)}
                  >
                    <Trash2 className="h-4 w-4 text-destructive" />
                  </Button>
                </div>
              </CardTitle>
              <CardDescription>
                FCK: {traco.resistenciaFck} MPa | Slump: {traco.slump} cm
              </CardDescription>
            </CardHeader>
            <CardContent className="space-y-4">
              <div className="space-y-2">
                <p className="text-sm font-medium text-muted-foreground">Componentes:</p>
                <div className="space-y-1">
                  {tracosDetalhes[traco.id]?.componentes?.map((comp, idx) => (
                    <div key={idx} className="flex items-center justify-between text-sm">
                      <span className="text-foreground">{comp.nomeMaterial}</span>
                      <Badge variant="secondary">{comp.quantidade} {comp.unidadeMedida}</Badge>
                    </div>
                  )) || (
                    <span className="text-sm text-muted-foreground">Nenhum componente cadastrado</span>
                  )}
                </div>
              </div>
              
              {custos[traco.id] !== undefined && (
                <div className="pt-4 border-t">
                  <div className="flex items-center justify-between">
                    <span className="text-sm font-medium text-muted-foreground">Custo Total:</span>
                    <span className="text-2xl font-bold text-primary">
                      R$ {custos[traco.id].toFixed(2)}
                    </span>
                  </div>
                  <p className="text-xs text-muted-foreground mt-1">por metro cúbico</p>
                </div>
              )}
            </CardContent>
          </Card>
          ))}
        </div>
      )}
    </div>
  );
};

export default Tracos;
