using System;

namespace WNMF.Common.Definition {
    public class ServiceRegistration {
        public ServiceRegistration(
            string name,
            Predicate<Type> typeCheck,
            object instance) {
            Name = name;
            TypeCheck = typeCheck;
            Instance = instance;
        }

        public object Instance { get; }

        public Predicate<Type> TypeCheck { get; }

        public string Name { get; }
    }
}