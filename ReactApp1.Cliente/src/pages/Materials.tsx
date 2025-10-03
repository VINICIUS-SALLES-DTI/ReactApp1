import { useState, useEffect } from "react";
import { Plus, Edit, Trash2, Loader2 } from "lucide-react";
import { Button } from "@/components/ui/button";
import { Card, CardContent, CardDescription, CardHeader, CardTitle } from "@/components/ui/card";
import { Dialog, DialogContent, DialogDescription, DialogHeader, DialogTitle, DialogTrigger } from "@/components/ui/dialog";
import { Input } from "@/components/ui/input";
import { Label } from "@/components/ui/label";
import { useToast } from "@/hooks/use-toast";
import { apiService, Material } from "@/services/api";

const Materials = () => {
  const { toast } = useToast();
  const [materials, setMaterials] = useState<Material[]>([]);
  const [loading, setLoading] = useState(true);
  const [isDialogOpen, setIsDialogOpen] = useState(false);
  const [editingMaterial, setEditingMaterial] = useState<Material | null>(null);
  const [formData, setFormData] = useState({
    nome: "",
    precoUnitario: "",
  });
  const [submitting, setSubmitting] = useState(false);

  useEffect(() => {
    loadMaterials();
  }, []);

  const loadMaterials = async () => {
    try {
      setLoading(true);
      const data = await apiService.getMateriais();
      setMaterials(data);
    } catch (error) {
      toast({
        title: "Erro ao carregar materiais",
        description: "Não foi possível carregar a lista de materiais.",
        variant: "destructive",
      });
    } finally {
      setLoading(false);
    }
  };

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    setSubmitting(true);
    
    try {
      const materialData = {
        nome: formData.nome,
        precoUnitario: parseFloat(formData.precoUnitario),
      };

      if (editingMaterial) {
        await apiService.updateMaterial(editingMaterial.id, materialData);
        toast({
          title: "Material atualizado",
          description: "O material foi atualizado com sucesso.",
        });
      } else {
        await apiService.createMaterial(materialData);
        toast({
          title: "Material cadastrado",
          description: "O material foi cadastrado com sucesso.",
        });
      }
      
      resetForm();
      await loadMaterials();
    } catch (error) {
      toast({
        title: "Erro",
        description: editingMaterial 
          ? "Não foi possível atualizar o material." 
          : "Não foi possível cadastrar o material.",
        variant: "destructive",
      });
    } finally {
      setSubmitting(false);
    }
  };

  const handleEdit = (material: Material) => {
    setEditingMaterial(material);
    setFormData({
      nome: material.nome,
      precoUnitario: material.precoUnitario.toString(),
    });
    setIsDialogOpen(true);
  };

  const handleDelete = async (id: number) => {
    try {
      await apiService.deleteMaterial(id);
      toast({
        title: "Material excluído",
        description: "O material foi excluído com sucesso.",
        variant: "destructive",
      });
      await loadMaterials();
    } catch (error) {
      toast({
        title: "Erro",
        description: "Não foi possível excluir o material.",
        variant: "destructive",
      });
    }
  };

  const resetForm = () => {
    setFormData({ nome: "", precoUnitario: "" });
    setEditingMaterial(null);
    setIsDialogOpen(false);
  };

  return (
    <div className="container mx-auto p-6 space-y-6">
      <div className="flex items-center justify-between">
        <div>
          <h1 className="text-3xl font-bold text-foreground">Materiais</h1>
          <p className="text-muted-foreground mt-1">Gerencie os materiais utilizados nos traços</p>
        </div>
        
        <Dialog open={isDialogOpen} onOpenChange={setIsDialogOpen}>
          <DialogTrigger asChild>
            <Button className="gap-2" onClick={() => setEditingMaterial(null)}>
              <Plus className="h-4 w-4" />
              Novo Material
            </Button>
          </DialogTrigger>
          <DialogContent>
            <DialogHeader>
              <DialogTitle>{editingMaterial ? "Editar Material" : "Novo Material"}</DialogTitle>
              <DialogDescription>
                {editingMaterial ? "Atualize as informações do material" : "Cadastre um novo material"}
              </DialogDescription>
            </DialogHeader>
            <form onSubmit={handleSubmit} className="space-y-4">
              <div className="space-y-2">
                <Label htmlFor="nome">Nome do Material</Label>
                <Input
                  id="nome"
                  value={formData.nome}
                  onChange={(e) => setFormData({ ...formData, nome: e.target.value })}
                  placeholder="Ex: Cimento"
                  required
                />
              </div>
              <div className="space-y-2">
                <Label htmlFor="precoUnitario">Preço Unitário (R$)</Label>
                <Input
                  id="precoUnitario"
                  type="number"
                  step="0.01"
                  value={formData.precoUnitario}
                  onChange={(e) => setFormData({ ...formData, precoUnitario: e.target.value })}
                  placeholder="0.00"
                  required
                />
              </div>
              <div className="flex gap-2 justify-end pt-4">
                <Button type="button" variant="outline" onClick={resetForm} disabled={submitting}>
                  Cancelar
                </Button>
                <Button type="submit" disabled={submitting}>
                  {submitting && <Loader2 className="mr-2 h-4 w-4 animate-spin" />}
                  {editingMaterial ? "Atualizar" : "Cadastrar"}
                </Button>
              </div>
            </form>
          </DialogContent>
        </Dialog>
      </div>

      {loading ? (
        <div className="flex items-center justify-center p-8">
          <Loader2 className="h-8 w-8 animate-spin" />
          <span className="ml-2">Carregando materiais...</span>
        </div>
      ) : (
        <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-4">
          {materials.map((material) => (
          <Card key={material.id} className="hover:shadow-lg transition-shadow">
            <CardHeader>
              <CardTitle className="flex items-center justify-between">
                {material.nome}
                <div className="flex gap-2">
                  <Button
                    size="icon"
                    variant="ghost"
                    onClick={() => handleEdit(material)}
                  >
                    <Edit className="h-4 w-4" />
                  </Button>
                  <Button
                    size="icon"
                    variant="ghost"
                    onClick={() => handleDelete(material.id)}
                  >
                    <Trash2 className="h-4 w-4 text-destructive" />
                  </Button>
                </div>
              </CardTitle>
              <CardDescription>
                Status: {material.disponivel ? "Disponível" : "Indisponível"}
              </CardDescription>
            </CardHeader>
            <CardContent>
              <div className="text-2xl font-bold text-primary">
                R$ {material.precoUnitario.toFixed(2)}
              </div>
              <p className="text-sm text-muted-foreground">preço unitário</p>
            </CardContent>
          </Card>
          ))}
        </div>
      )}
    </div>
  );
};

export default Materials;
