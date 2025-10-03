import { Layers, Package } from "lucide-react";
import { Button } from "@/components/ui/button";
import { Card, CardContent, CardDescription, CardHeader, CardTitle } from "@/components/ui/card";
import { useNavigate } from "react-router-dom";

const Index = () => {
  const navigate = useNavigate();

  const features = [
    {
      icon: Package,
      title: "Materiais",
      description: "Cadastre e gerencie todos os materiais utilizados",
      action: () => navigate("/materials"),
      color: "text-primary",
    },
    {
      icon: Layers,
      title: "Traços",
      description: "Crie e organize diferentes traços de concreto",
      action: () => navigate("/tracos"),
      color: "text-secondary",
    },
  ];

  return (
    <div className="min-h-screen bg-background">
      <div className="container mx-auto px-4 py-16 space-y-12">
        <div className="text-center space-y-4 max-w-3xl mx-auto">
          <h1 className="text-5xl font-bold bg-gradient-to-r from-primary to-secondary bg-clip-text text-transparent">
            Sistema de Cálculo de Traço
          </h1>
          <p className="text-xl text-muted-foreground">
            Gerencie materiais, crie traços e calcule custos de concreto de forma rápida e precisa
          </p>
        </div>

        <div className="grid grid-cols-1 md:grid-cols-2 gap-6 max-w-4xl mx-auto">
          {features.map((feature, index) => (
            <Card 
              key={index} 
              className="hover:shadow-xl transition-all duration-300 cursor-pointer border-2 hover:border-primary/50"
              onClick={feature.action}
            >
              <CardHeader className="space-y-4">
                <div className={`w-12 h-12 rounded-lg bg-gradient-to-br from-primary/10 to-secondary/10 flex items-center justify-center ${feature.color}`}>
                  <feature.icon className="h-6 w-6" />
                </div>
                <CardTitle className="text-xl">{feature.title}</CardTitle>
                <CardDescription className="text-base">
                  {feature.description}
                </CardDescription>
              </CardHeader>
              <CardContent>
                <Button 
                  className="w-full" 
                  variant="outline"
                  onClick={feature.action}
                >
                  Acessar
                </Button>
              </CardContent>
            </Card>
          ))}
        </div>

        <div className="text-center space-y-4 pt-12">
          <div className="inline-block px-6 py-3 bg-muted rounded-lg">
            <p className="text-sm text-muted-foreground">
              💡 Comece cadastrando seus materiais e depois crie os traços
            </p>
          </div>
        </div>
      </div>
    </div>
  );
};

export default Index;
